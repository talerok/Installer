using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class ExceptionGenerator
    {
        public static string Generate(string constructor, params string[] args)
        {
            return $"throw {ObjectGenerator.Generate(null, constructor, args)}";
        }
    }
}
