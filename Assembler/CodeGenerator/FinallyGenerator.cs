using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    static class FinallyGenerator
    {
        public static string Generate(string body)
        {
            var res = new StringBuilder();
            res.AppendLine("finally {");
            res.AppendLine(body);
            res.AppendLine("}");
            return res.ToString();
        }
    }
}
