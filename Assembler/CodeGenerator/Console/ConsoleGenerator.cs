using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.Console
{
    class ConsoleGenerator
    {
        private List<string> _nameSpaces = new List<string> {
            "System",
            "System.IO",
            "System.Net",
            "System.Linq",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Collections.Generic",
            "System.Reflection"
        };

        private string _code;
        private string _rootNameSpace;
        private IDictionary<string, string> _localLibs;
        
        public ConsoleGenerator(string rootNameSpace, IEnumerable<string> nameSpaces, IDictionary<string, string> localLibs, string code)
        {
            _localLibs = localLibs;
            _nameSpaces.AddRange(nameSpaces);
            _code = code;
            _rootNameSpace = rootNameSpace;
        }

        public string Generate()
        {
            try
            {
                var res = new StringBuilder();

                var libResolveCode = ResourceResolverGenerator.Generate(_localLibs.Keys);

                res.AppendLine(NameSpacesGenerator.Generate(_nameSpaces));
                res.AppendLine(NameSpaceGenerator.Generate(_rootNameSpace,
                    ClassGenerator.Generate(new string[] { "public", "static" }, "Programm",
                        $"{libResolveCode.MethodCode}\n" +
                        $"{MethodGenerator.Generate(new string[] { "static" }, "void", "Progrm", new string[] { }, _code)}\n" +
                        MethodGenerator.Generate(new string[] { "static" }, "void", "Main", new string[] { "string[] args" },
                            $"{libResolveCode.InMainCode}\n" +
                            "Progrm();"
                        )
                    )
                ));
                return res.ToString();
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException(ex.Message);
            }
        }

    }
}
