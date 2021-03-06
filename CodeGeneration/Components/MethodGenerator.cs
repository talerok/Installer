﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class MethodGenerator
    {
        public static string Generate(IEnumerable<string> modifiers, string methodType, string MethodName, IEnumerable<string> prms, string body)
        {
            var res = new StringBuilder();
            res.AppendLine($"{string.Join(" ", modifiers)} {methodType} {MethodName}({string.Join(", ", prms)}) {{");
            res.AppendLine(body);
            res.Append("}");
            return res.ToString();
        }
    }
}
