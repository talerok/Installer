using CodeGeneration;
using CodeGeneration.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.Form
{
    public class FormGlobal
    {
        public string NameSpace { get; }
        public IEnumerable<string> NameSpaces { get; }
        public string ClassName { get; }
        public string ClassCode { get; }
        public string MainCode { get; }
        
        public FormGlobal(IEnumerable<string> namespaces, string nameSpace, string className, string classCode, string mainCode)
        {
            NameSpace = nameSpace;
            NameSpaces = namespaces;
            ClassCode = classCode;
            ClassName = className;
            MainCode = mainCode;
        }
    }


    class FormGenerator
    {
        private List<string> _nameSpaces = new List<string> {
            "System",
            "System.IO",
            "System.Net",
            "System.Linq",
            "System.Text",
            "System.Text.RegularExpressions",
            "System.Collections.Generic",
            "System.Reflection",
            "System.Windows.Forms",
            "System.Threading"
        };

        private string _code;
        private string _rootNameSpace;
        private IDictionary<string, string> _localLibs;

        public FormGenerator(string rootNameSpace, IEnumerable<string> nameSpaces, IDictionary<string, string> localLibs, string code)
        {
            _localLibs = localLibs;
            _nameSpaces.AddRange(nameSpaces);
            _code = code;
            _rootNameSpace = rootNameSpace;
        }

        public string Generate()
        {
            var libResolveCode = ResourceResolverGenerator.Generate(_rootNameSpace, _localLibs.Keys);

            var classBody = new StringBuilder();
            classBody.AppendLine(_code);
            classBody.AppendLine(libResolveCode.MethodCode);

            var mainBody = new StringBuilder();

            var global = new FormGlobal(_nameSpaces, _rootNameSpace, "Programm", classBody.ToString(), libResolveCode.InMainCode);

            return CodeGeneration.CodeGenerator.GenerateFromFile(@"Templates\Form.cstemplate", global).Result;
        }
    }
}
