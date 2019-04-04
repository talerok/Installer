using Assembler.CodeGenerator;
using Assembler.CodeGenerator.AdvancedForm;
using Assembler.CodeGenerator.SimpleForm;
using Assembler.CodeGenerator.Uninstaller;
using Assembler.Compiler;
using Assembler.Compiler.Interfaces;
using Assembler.Compiler.WinApp;
using Assembler.InstallConfig;
using Common;
using Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembler
{
    public class AssemblerEventArgs : EventArgs
    {
        public string Message { get; }

        public AssemblerEventArgs(string msg)
        {
            Message = msg;
        }
    }

    public class InstallerAssembler
    {
        private const string _simpleType = "simple";
        private const string _advancedType = "advanced";

        private readonly string[] _namespaces = new[] {
                    "InstallerLib.Progress",
                    "InstallerLib.Installer.InstallCheck",
                    "InstallerLib.Installer.InstallCommand",
                    "InstallerLib.Installer.InstallCommand.Interfaces",
                    "InstallerLib.Installer.InstallCommand.Registry",
                    "InstallerLib.Installer.InstallCommand.Unpacking",
                    "InstallerLib.Installer.InstallInfo",
                    "InstallerLib.Installer.InstallCommand.Directory",
                    "InstallerLib.Installer.InstallCommand.ShortCut",
                    "InstallerLib.Uninstaller"
                };

        private static void _copyToDir(string sourceDir, string targetDir)
        {
            System.IO.Directory.CreateDirectory(targetDir);

            foreach (var file in System.IO.Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in System.IO.Directory.GetDirectories(sourceDir))
                _copyToDir(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

        private string _originalBuildPath;
        private Config _config;

        public event EventHandler<AssemblerEventArgs> EventHandler;
        private Iterator _saveVersionIterator;

        public InstallerAssembler(Config config)
        {
            _originalBuildPath = config.BuildPath;
            _config = (Config)config.Clone();
            _config.BuildPath = VersionPath.Generate(_config.AppName, _config.Version);

            _saveVersionIterator = Iterator.Factory.Create(_saveVersion, 1);
        }

        private void _print(string msg)
        {
            if (EventHandler != null)
                EventHandler.Invoke(this, new AssemblerEventArgs(msg));
        }

        private void _addLocalLibs(ICompiler compiler)
        {
            foreach (var lib in Directory.GetFiles("Libs", "*.dll"))
                compiler.AddLocalLib(lib);
        }

        private ICompiler _getCompiler(BuildType buildType)
        {
            var advCodeInfo = new AdvancedFormInstallCodeGenerator(_config, buildType).GetCode();
            var res = new WinAppCompiler(_config.FrameworkVer, _namespaces, advCodeInfo.Code, advCodeInfo.Resources, _config.IconPath);
            _addLocalLibs(res);
            return res;
        }

        private void _assembleUninstaller()
        {
            _print(Resources.UninstallerAssemblingMessageText);
            var unstallCode = new UninstallerCodeGenerator(_config);
            var uninstallerCompiler = new WinAppCompiler(_config.FrameworkVer, _namespaces, unstallCode.Generate(), new Dictionary<string, byte[]>(), _config.IconPath);
            var path = $@"{_config.BuildPath}\uninstaller.exe";
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            _addLocalLibs(uninstallerCompiler);
            uninstallerCompiler.Compile(path);
        }

        private string _formatTemplate(string template, object obj)
        {
            var properties = obj.GetType().GetProperties();
            var res = template;

            foreach (var property in properties)
            {
                if (property.PropertyType.Name != "String")
                    continue;

                var val = property.GetValue(obj) as string;
                if (val == null)
                    continue;

                res = res.Replace($"[{property.Name}]", val);
            }
            return res;
        }

        private string _generateFileNameByTemplate(BuildType buildType)
        {
            var res = buildType == BuildType.Major ? _config.MajorConfig.FileNameTemplate : _config.MinorConfig.FileNameTemplate;

            res = _formatTemplate(res, _config);

            if (buildType == BuildType.Major)
                res = _formatTemplate(res, _config.MajorConfig);
            else
                res = _formatTemplate(res, _config.MinorConfig);
           
            return res;
        }

        private string _generateOutputPath(BuildType buildType)
        {
            return $@"Output\{_config.AppName}\{_config.Version}\{(buildType == BuildType.Major ? "Major" : "Minor")}\{_generateFileNameByTemplate(buildType)}";
        }

        private void _saveVersion()
        {
            _print(Resources.CopyBuildFilesMessageText);

            if (Directory.Exists(_config.BuildPath))
                Directory.Delete(_config.BuildPath, true);
            _copyToDir(_originalBuildPath, _config.BuildPath);
            _assembleUninstaller();
        }

        public void Assemble(BuildType buildType)
        {
            try
            {
                if (!Framework.Check(_config.FrameworkVer))
                    throw new Exception(Resources.IncorrectFrameworkVersionMessageText);

                if (!Framework.Exists(_config.FrameworkVer))
                    throw new Exception(Resources.FrameworkNotFoundMessageText.GetFormated(_config.FrameworkVer));

                var output = _generateOutputPath(buildType);
                var dir = Path.GetDirectoryName(output);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _saveVersionIterator.Do();

                var compiler = _getCompiler(buildType);
  
                _print(Resources.AssemblingMessageText.GetFormated(output));
                compiler.Compile(output);
                _print(Resources.AssemblingFinishMessageText);
            }
            catch (CompilerException ex)
            {
                _print(Resources.CompilationErrorMessageText);
                _print(ex.Message);
            }
            catch (CodeGeneratorException ex)
            {
                _print(Resources.CodeGenerationErrorMessageText);
                _print(ex.Message);
            }
            catch (Exception ex)
            {
                _print(Resources.AssemblerErrorMessageText);
                _print(ex.Message);
            }
        }

    }
}
