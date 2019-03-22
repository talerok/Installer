using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class StringGenerator
    {
        public static string Generate(string body)
        {
            return $@"@""{body}""";
        }
    }
}
