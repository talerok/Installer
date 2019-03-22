using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    static class ExceptionGenerator
    {
        public static string Generate(string constructor, params string[] args)
        {
            return $"throw {ObjectGenerator.Generate(null, constructor, args)}";
        }
    }
}
