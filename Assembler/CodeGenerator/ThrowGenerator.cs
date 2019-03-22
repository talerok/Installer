using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class ThrowGenerator
    {
        public static string Generate(string exceptionClass, params string[] prms)
        {
            return $@"throw new {exceptionClass}({string.Join(",", prms)});";
        }
    }
}
