using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class CatchGenerator
    {
        public static string Generate(string excpetionClass, string exceptionName, string body)
        {
            var res = new StringBuilder();
            res.AppendLine($"catch ({excpetionClass} {exceptionName}) {{");
            res.AppendLine(body);
            res.Append("}");
            return res.ToString();
        }
    }
}
