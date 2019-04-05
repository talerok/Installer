using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Components
{
    public static class ResourceResolverGenerator
    {
        public static (string MethodCode, string InMainCode) Generate(string nmsp, IEnumerable<string> libs)
        {
            var resolveMethodBody = new StringBuilder();
            var getResxMethodBody  = new StringBuilder();
            var methodCodeRes = new StringBuilder();
            var inMainCodeRes = "AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(_resolveAssembly);";

            getResxMethodBody.AppendLine($@"System.Resources.ResourceManager rm = new System.Resources.ResourceManager(""{nmsp}.g"", Assembly.GetExecutingAssembly());");
            getResxMethodBody.Append(@"return rm.GetObject(name);");

            foreach(var lib in libs)
            {
                resolveMethodBody.AppendLine($@"if(args.Name.Contains(""{lib}""))");
                resolveMethodBody.AppendLine($@"return Assembly.Load((byte[])_getResxByName(""{lib}""));");
            }
            resolveMethodBody.Append("return null;");

            methodCodeRes.AppendLine(MethodGenerator.Generate(new string[] {"private", "static"}, "object", "_getResxByName", new string[] {"string name"}, getResxMethodBody.ToString()));
            methodCodeRes.AppendLine(MethodGenerator.Generate(new string[] { "private", "static" }, "Assembly", "_resolveAssembly", new string[] { "object sender", "ResolveEventArgs args" }, resolveMethodBody.ToString()));

            return (methodCodeRes.ToString(), inMainCodeRes);
        }
    }
}
