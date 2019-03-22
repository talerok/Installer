using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    static class TryGenerator
    {
        public static string Generate(string body)
        {
            var res = new StringBuilder();
            res.AppendLine("try {");
            res.AppendLine(body);
            res.AppendLine("}");
            return res.ToString();
        }
    }
}
