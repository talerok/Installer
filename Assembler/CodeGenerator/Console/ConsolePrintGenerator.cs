using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.Console
{
    class ConsolePrintGenerator
    {
        public static string Generate(string msg)
        {
            return $@"Console.WriteLine({msg});";
        }
    }
}
