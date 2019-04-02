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
            assembler.Assemble();
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey();
        }
    }
}
