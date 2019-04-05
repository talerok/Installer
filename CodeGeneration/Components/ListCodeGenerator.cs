using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration.Components
{
    public static class ListCodeGenerator
    {
        public static string Generate(string name, string type, IEnumerable<string> body)
        {
            var res = new StringBuilder();
            if (name != null)
                res.Append($"var {name} = ");
            res.Append($"new List<{type}>()");
            if (body.Any())
            {
                res.AppendLine(" {");
                res.AppendLine(string.Join(",\n", body));
                res.Append(" }");
            }
            if (name != null)
                res.Append(";");
            return res.ToString();
        }
    }
}
