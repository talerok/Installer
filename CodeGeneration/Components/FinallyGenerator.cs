using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class FinallyGenerator
    {
        public static string Generate(string body)
        {
            var res = new StringBuilder();
            res.AppendLine("finally {");
            res.AppendLine(body);
            res.Append("}");
            return res.ToString();
        }
    }
}
