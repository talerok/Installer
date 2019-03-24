using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.SimpleForm
{
    class SimpleFormGenerator
    {
        private List<string> _nameSpaces = new List<string> {
            "System",
            "System.IO",
            "System.Net",
            "System.Linq",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Collections.Generic",
            "System.Reflection",
            "System.Windows.Forms"
        };

        private string _code;
        private string _rootNameSpace;
        private IDictionary<string, string> _localLibs;

        public SimpleFormGenerator(string rootNameSpace, IEnumerable<string> nameSpaces, IDictionary<string, string> localLibs, string code)
        {
            _localLibs = localLibs;
            _nameSpaces.AddRange(nameSpaces);
            _code = code;
            _rootNameSpace = rootNameSpace;
        }

        public string Generate()
        {
            try
            {
                var res = new StringBuilder();

                var libResolveCode = ResourceResolverGenerator.Generate(_rootNameSpace, _localLibs.Keys);

                res.AppendLine(NameSpacesGenerator.Generate(_nameSpaces));

                var mainBody = new StringBuilder();
                mainBody.AppendLine(libResolveCode.InMainCode);
                mainBody.AppendLine("Application.EnableVisualStyles();");
                mainBody.AppendLine("Application.SetCompatibleTextRenderingDefault(false);");
                mainBody.AppendLine("Application.Run(new Form1());");

                var classBody = new StringBuilder();
                classBody.AppendLine(_code);
                classBody.AppendLine(libResolveCode.MethodCode);
                classBody.AppendLine("[STAThread]");
                classBody.AppendLine(MethodGenerator.Generate(new string[] { "static" }, "void", "Main", new string[] { }, mainBody.ToString()));
                

                res.AppendLine(NameSpaceGenerator.Generate(_rootNameSpace,
                    ClassGenerator.Generate(new string[] { "public", "static" }, "Program", classBody.ToString())));
                return res.ToString();
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException(ex.Message);
            }
        }
    }
}
