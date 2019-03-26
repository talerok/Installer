using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

using Assembler.Compiler;

using Assembler.CodeGenerator;
using System.IO;
using Assembler.InstallConfig;
using Assembler.CodeGenerator.SimpleForm;
using Assembler.Compiler.WinApp;
using Assembler.Compiler.Interfaces;

namespace Assembler
{
    class Program
    {
        private const string _simpleType = "simple";

        private static bool _checkFrameworkVersion(string ver)
        {
            switch (ver)
            {
                case "3.5":
                case "4.0":
                case "4.5":
                case "4.5.1":
                case "4.5.2":
                case "4.6":
                case "4.6.1":
                case "4.6.2":
                case "4.7":
                case "4.7.1":
                case "4.7.2":
                    return true;
                default:
                    return false;

            }
        }

        private static bool _checkFrameworkVersionExists(string ver)
        {
            var folder = $@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v{ver}\";
            return Directory.Exists(folder);
        }

        static void Main(string[] args)
        { 
            var namespaces = new[] {
                    "InstallerLib.Installer.InstallCheck",
                    "InstallerLib.Installer.InstallCommand",
                    "InstallerLib.Installer.InstallCommand.Interfaces",
                    "InstallerLib.Installer.InstallCommand.Registry",
                    "InstallerLib.Installer.InstallCommand.Unpacking",
                    "InstallerLib.Installer.InstallInfo",
                    "InstallerLib.Installer.InstallCommand.Directory"
                };

            try
            {
                var config = new JSONConfigReader("config.json").Read();

                var dir = Path.GetDirectoryName(config.OutputPath);
                var file = Path.GetFileName(config.OutputPath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                ICompiler compiler = null;

                

                if (!_checkFrameworkVersion(config.FrameworkVer))
                    throw new Exception("Неизвестная версия .Net Framework");

                if (!_checkFrameworkVersionExists(config.FrameworkVer))
                    throw new Exception($@"Версия .Net Framework {config.FrameworkVer} не найдена на компьютере");

                switch (config.Type)
                {
                    case _simpleType:
                        var appCodeInfo = new SimpleFormInstallCodeGenerator(config).GetCode();
                        compiler = new WinAppCompiler(config.FrameworkVer, namespaces, appCodeInfo.Code, appCodeInfo.Resources);
                        break;
                    default:
                        throw new Exception("Неизвестный тип инсталятора");
                }

               

                compiler.AddLocalLib("DotNetZip");
                compiler.AddLocalLib("InstallerLib");

                Console.WriteLine($"Сборка инсталятора {file}");
                compiler.Compile(dir, file);
                Console.WriteLine("Инсталятор собран");
            }
            catch (CompilerException ex)
            {
                Console.WriteLine("Ошибка компиляции:");
                Console.WriteLine(ex.Message);
            }
            catch (CodeGeneratorException ex)
            {
                Console.WriteLine("Ошибка генерации кода:");
                Console.WriteLine(ex.Message);
            }catch (Exception ex)
            {
                Console.WriteLine("Ошибка упаковщика:");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
    }
}
