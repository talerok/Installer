using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class LambdaGenerator
    {
        public static string Generate(string body, params string[] args)
        {
            return $@"({string.Join(",",args)}) => {{{body}}}";
        }
    }
}
