using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class ClassGenerator
    {
        public static string Generate(IEnumerable<string> modifiers, string name, string body)
        {
            var res = new StringBuilder();
            res.AppendLine($"{string.Join(" ", modifiers)} class {name} {{");
            res.AppendLine(body);
            res.AppendLine("}");
            return res.ToString();
        }
    }
}
