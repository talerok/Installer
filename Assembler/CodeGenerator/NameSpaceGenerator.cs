using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class NameSpaceGenerator
    {
        public static string Generate(string name, string body)
        {
            var res = new StringBuilder();
            res.AppendLine($"namespace {name} {{");
            res.AppendLine(body);
            res.AppendLine("}");
            return res.ToString();
        }
    }
}
