using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.Console
{
    class ConsolePrintGenerator
    {
        public static string Generate(string msg, bool quotes = false)
        {
            if(quotes)
                return $@"Console.WriteLine(""{msg}"");";
            else
                return $@"Console.WriteLine({msg});";
        }
    }
}
