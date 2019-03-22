using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    static class ForeachGenerator
    {
        public static string Generate(string elemName, string arrayName, string body)
        {
            var res = new StringBuilder();
            res.AppendLine($"foreach(var {elemName} in {arrayName}) {{");
            res.AppendLine(body);
            res.AppendLine("}");
            return res.ToString();
        }
    }
}
