using Assembler.CodeGenerator;
using Assembler.CodeGenerator.AdvancedForm;
using Assembler.CodeGenerator.SimpleForm;
using Assembler.CodeGenerator.Uninstaller;
using Assembler.Compiler;
using Assembler.Compiler.Interfaces;
using Assembler.Compiler.WinApp;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public InstallerAssembler(Config config)
        {
            _originalBuildPath = config.BuildPath;
            _config = (Config)config.Clone();
            _config.BuildPath = $@"{_config.VersionsFolderPath}\{_config.Version}"; 
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

        private ICompiler _getCompiler()
        {
            ICompiler res;
            switch (_config.Type)
            {
                case _simpleType:
                    var appCodeInfo = new SimpleFormInstallCodeGenerator(_config).GetCode();
                    res = new WinAppCompiler(_config.FrameworkVer, _namespaces, appCodeInfo.Code, appCodeInfo.Resources, _config.IconPath);
                    break;
                case _advancedType:
                    var advCodeInfo = new AdvancedFormInstallCodeGenerator(_config).GetCode();
                    res = new WinAppCompiler(_config.FrameworkVer, _namespaces, advCodeInfo.Code, advCodeInfo.Resources, _config.IconPath);
                    break;
                default:
                    throw new Exception("Неизвестный тип (Type)");
            }
            _addLocalLibs(res);
            return res;
        }

        private void _assembleUninstaller()
        {
            if (!string.IsNullOrEmpty(_config.ForVersion))
                return;

            _print("Сборка деинсталятора");
            var unstallCode = new UninstallerCodeGenerator(_config);
            var uninstallerCompiler = new WinAppCompiler(_config.FrameworkVer, _namespaces, unstallCode.Generate(), new Dictionary<string, byte[]>(), _config.IconPath);
            var path = $@"{_config.BuildPath}\{_config.UninstallerPath}";
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            _addLocalLibs(uninstallerCompiler);
            uninstallerCompiler.Compile(path);
        }

        public void Assemble()
        {
            try
            {
                var dir = Path.GetDirectoryName(_config.OutputPath);

                if (!Framework.Check(_config.FrameworkVer))
                    throw new Exception("Неизвестная версия .Net Framework");

                if (!Framework.Exists(_config.FrameworkVer))
                    throw new Exception($@"Версия .Net Framework {_config.FrameworkVer} не найдена на компьютере");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                _print("Копирование файлов сборки");

                if (Directory.Exists(_config.BuildPath))
                    Directory.Delete(_config.BuildPath, true);
                _copyToDir(_originalBuildPath, _config.BuildPath);

                _assembleUninstaller();

                var compiler = _getCompiler();
  
                _print($"Сборка {_config.OutputPath}");
                compiler.Compile(_config.OutputPath);
                _print("Сборка прошла успешно");
            }
            catch (CompilerException ex)
            {
                _print("Ошибка компиляции:");
                _print(ex.Message);
            }
            catch (CodeGeneratorException ex)
            {
                _print("Ошибка генерации кода:");
                _print(ex.Message);
            }
            catch (Exception ex)
            {
                _print("Ошибка упаковщика:");
                _print(ex.Message);
            }
        }

    }
}
