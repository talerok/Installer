using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeGeneration.Components
{
    public static class NameSpacesGenerator
    {

        public static string Generate(IEnumerable<string> namespaces)
        {
            return string.Join("\n", namespaces.Select(x => $"using {x};"));
        }

        public static string Generate(params string[] namespaces)
        {
            return Generate(namespaces.ToList());
        }
    }
}
