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
using Assembler.CodeGenerator.AdvancedForm;
using Assembler.CodeGenerator.Uninstaller;

namespace Assembler
{
    class Program
    {
       
        static void Main(string[] args)
        { 
            var config = new JSONConfigReader("config.json").Read();
            var assembler = new InstallerAssembler(config);
            assembler.EventHandler += (o, e) => Console.WriteLine(e.Message);

            Console.WriteLine("Введите тип установщика (major, minor, both): ");
            string type = Console.ReadLine().ToLower();

            switch (type)
            {
                case "major":
                    assembler.Assemble(BuildType.Major);
                    break;
                case "minor":
                    assembler.Assemble(BuildType.Minor);
                    break;
                case "both":
                    assembler.Assemble(BuildType.Major);
                    assembler.Assemble(BuildType.Minor);
                    break;
            }
            
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
    }
}
