using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

using Assembler.Compiler;
using Assembler.Compiler.Console;

using Assembler.CodeGenerator;
using Assembler.CodeGenerator.Console;
using System.IO;

namespace Assembler
{
    class Program
    {
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
                Console.WriteLine("Введите путь:");
                var path = Console.ReadLine();

                var dir = Path.GetDirectoryName(path);
                var file = Path.GetFileName(path);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var codeInfo = new JSONConsoleInstallCodeGenerator("config.json").GetCode();
                var compiler = new ConsoleCompiler("3.5", namespaces, codeInfo.Code, codeInfo.Resources);
                compiler.AddLocalLib("DotNetZip");
                compiler.AddLocalLib("InstallerLib");

                Console.WriteLine($"Сборка инсталятора {file}");
                compiler.Compile(dir, file);
                Console.WriteLine("Инсталятор собран");
            }
            catch (CompilerException ex)
            {
                Console.WriteLine("Ошибка компиляции");
                Console.WriteLine(ex.Message);
            }
            catch (CodeGeneratorException ex)
            {
                Console.WriteLine("Ошибка генерации кода");
                Console.WriteLine(ex.Message);
            }catch (Exception ex)
            {
                Console.WriteLine("Ошибка");
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
    }
}
