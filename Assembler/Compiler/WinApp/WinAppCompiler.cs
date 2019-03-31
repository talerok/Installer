using Assembler.CodeGenerator;
using Assembler.CodeGenerator.Form;
using Assembler.CodeGenerator.SimpleForm;
using Assembler.Compiler.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace Assembler.Compiler.WinApp
{
    class WinAppCompiler : ICompiler
    {
        private string[] _namespaces;
        private string _code;
        private List<string> _frameworkLibs = new List<string>();
        private Dictionary<string, string> _localLibs = new Dictionary<string, string>();
        private IDictionary<string, byte[]> _resources;
        private string _iconPath = null;

        private const string _rootNameSpace = "WinApp";

        public WinAppCompiler(string frameworkVer, string[] namespaces, string code, IDictionary<string, byte[]> resources, string iconPath = null)
        {
            _namespaces = namespaces;
            _code = code;
            _resources = resources;
            _iconPath = iconPath;
            AddFrameworkLib(frameworkVer, "mscorlib");
            AddFrameworkLib(frameworkVer, "System");
            AddFrameworkLib(frameworkVer, "System.Core");
            AddFrameworkLib(frameworkVer, "System.Drawing");
            AddFrameworkLib(frameworkVer, "System.Windows.Forms");
        }

        private static string runtimePath(string frameworkVersion, string libName) => $@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v{frameworkVersion}\Profile\Client\{libName}.dll";

        private IEnumerable<MetadataReference> _generateReferences() =>
            _frameworkLibs.Concat(_localLibs.Values).Select(x => MetadataReference.CreateFromFile(x));

        private CSharpCompilationOptions _generateCompilationOptions() => new CSharpCompilationOptions(OutputKind.WindowsApplication)
                    .WithOverflowChecks(false).WithOptimizationLevel(OptimizationLevel.Release);

        private static SyntaxTree _parse(string text, CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options);
        }

        public void AddLocalLib(string libName)
        {
            _localLibs.Add(Path.GetFileName(libName).Replace(".dll", ""), libName);
        }

        public void AddFrameworkLib(string frameworkVer, string libName)
        {
            _frameworkLibs.Add(runtimePath(frameworkVer, libName));
        }

        private IEnumerable<ResourceDescription> _generateResources(IDictionary<string, byte[]> resources)
        {

            List<ResourceDescription> resourceDescriptions = new List<ResourceDescription>();
            string resourcePath = string.Format("{0}.g.resources", _rootNameSpace);
            ResourceWriter rsWriter = new ResourceWriter(resourcePath);

            foreach (var resource in resources)
                rsWriter.AddResource(resource.Key, resource.Value);

            rsWriter.Generate();
            rsWriter.Close();

            var resourceDescription = new ResourceDescription(
               string.Format("{0}.g.resources", _rootNameSpace),
               () => File.OpenRead(resourcePath),
               true);
            resourceDescriptions.Add(resourceDescription);

            return resourceDescriptions;
        }

        private static IDictionary<string, byte[]> _generateLocalLibsResources(IDictionary<string, string> libs)
        {
            return libs.Select(x => KeyValuePair.Create(x.Key, File.ReadAllBytes(x.Value))).ToDictionary(x => x.Key, x => x.Value);
        }


        public void Compile(string path)
        {

            var generator = new FormGenerator(_rootNameSpace, _namespaces, _localLibs, _code);
            var resources = _generateResources(_generateLocalLibsResources(_localLibs).Concat(_resources).ToDictionary(x => x.Key, x => x.Value));

            try
            {
                var code = generator.Generate();

                if (File.Exists("listing.txt"))
                    File.Delete("listing.txt");

                File.WriteAllText("listing.txt", ListingGenerator.GenerateCodeLisning(code, false));

                var parsedSyntaxTree = _parse(code, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7));
                var compilation
                   = CSharpCompilation.Create(_rootNameSpace, new SyntaxTree[] { parsedSyntaxTree }, _generateReferences(), _generateCompilationOptions());

                EmitResult res;

                using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    res = compilation.Emit(
                        peStream: fstream,
                        manifestResources: resources,
                        win32Resources: _iconPath != null ? compilation.CreateDefaultWin32Resources(true, false, null, File.OpenRead(_iconPath)) : null
                    );
                }

                if (!res.Success)
                    throw new CompilerException($"\n\n{ListingGenerator.GenerateCodeLisning(code)}\n\n{string.Join('\n', res.Diagnostics.Select(x => x.ToString()))}");
            }
            catch (CodeGeneratorException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CompilerException(ex.Message);
            }
        }
    }
}
