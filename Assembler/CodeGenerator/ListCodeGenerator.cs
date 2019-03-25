using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator
{
    static class ListCodeGenerator
    {
        public static string Generate(string name, string type, IEnumerable<string> body)
        {
            var res = new StringBuilder();
            if (name != null)
                res.Append($"var {name} = ");
            res.AppendLine($"new List<{type}>()");
            if (body.Any())
            {
                res.AppendLine(" {");
                res.AppendLine(string.Join(",\n", body));
                res.Append(" }");
            }
            if (name != null)
                res.AppendLine(";");
            return res.ToString();
        }
    }
}
