﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class ClassGenerator
    {
        public static string Generate(IEnumerable<string> modifiers, string name, string body, params string[] parents)
        {
            var res = new StringBuilder();
            res.Append($"{string.Join(" ", modifiers)} class {name} ");
            if (parents.Length > 0)
                res.Append($": {string.Join(", ", parents)}");
            res.AppendLine("{");
            res.AppendLine(body);
            res.Append("}");
            return res.ToString();
        }
    }
}
