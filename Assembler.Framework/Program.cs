using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Framework
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
