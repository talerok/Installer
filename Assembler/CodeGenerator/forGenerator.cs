using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class ForGenerator
    {
        public static string Generate(string a, string b, string c, string body)
        {
            var res = new StringBuilder();
            res.AppendLine($"for({a};{b};{c}) {{");
            res.AppendLine(body);
            res.AppendLine("}");
            return res.ToString();
        }
    }
}
