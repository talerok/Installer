using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    static class ObjectGenerator
    {
        public static string Generate(string name, string constructor, params string[] args)
        {
            var res = new StringBuilder();
            if (name != null)
                res.Append($"var {name} = ");
            res.Append($"new {constructor}(");
            if (args.Length > 0)
                res.Append(string.Join(",", args));

            if (name != null)
                res.AppendLine(");");
            else
                res.Append(")");
            return res.ToString();
        }
    }
}
