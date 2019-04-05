using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class ThrowGenerator
    {
        public static string Generate(string exceptionClass, params string[] prms)
        {
            return $@"throw new {exceptionClass}({string.Join(",", prms)});";
        }
    }
}
