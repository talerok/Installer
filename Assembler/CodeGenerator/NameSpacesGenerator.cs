using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class NameSpacesGenerator
    {

        public static string Generate(IEnumerable<string> namespaces)
        {
            StringBuilder res = new StringBuilder();
            foreach (var nsps in namespaces)
                res.AppendLine($"using {nsps};");
            return res.ToString();
        }

        public static string Generate(params string[] namespaces)
        {
            return Generate(namespaces);
        }
    }
}
