using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class StringGenerator
    {
        public static string Generate(string body, bool withDot = true)
        {
            if(withDot)
                return $@"@""{body}""";
            else
                return $@"""{body}""";
        }
    }
}
