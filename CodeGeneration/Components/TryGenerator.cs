using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class TryGenerator
    {
        public static string Generate(string body)
        {
            var res = new StringBuilder();
            res.AppendLine("try {");
            res.AppendLine(body);
            res.Append("}");
            return res.ToString();
        }
    }
}
