using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace CodeGeneration
{
    public class ScriptGlobal<T>
    {
        public T Global { get; }

        public string GenerateFromFile<E>(string path, E global)
        {
            return CodeGeneration.CodeGenerator.GenerateFromFile(path, global).Result;
        }

        public ScriptGlobal(T global)
        {
            Global = global;
        }
    }
   
    public static class CodeGenerator
    {
        private const string _blockPattern = "//-+?\\[GENERATE-START\\]-+\r?\n?([\\w\\W]+?)//-+?\\[GENERATE-END\\]-+\r?\n?";
        private const string _commentPattern = @"/\*\W*[GENERATE]([\w\W]*?)\*/";
        private const string _funcPattern = @"<- *\{([\w\W]*)\}";

        private static readonly string[] _nameSpaces =
        {
            "System",
            "System.Text",
            "System.Collections.Generic",
            "System.Linq",
            "CodeGeneration.Components"
        };

        private static async Task<string> _compileFuncs<T>(IEnumerable<string> funcs, T global)
        {
            var code = new StringBuilder();
            code.AppendLine(Components.NameSpacesGenerator.Generate(_nameSpaces));
            code.AppendLine("var funcs = new List<Func<string>>();");
            foreach (var func in funcs)
                code.AppendLine($@"funcs.Add({Components.LambdaGenerator.Generate(func, new string[] { })});");

            code.AppendLine(@"String.Join(""\n"", funcs.Select(x => x()))");

            return await CSharpScript.EvaluateAsync<string>(
                code: code.ToString(),
                globals: new ScriptGlobal<T>(global),
                globalsType: typeof(ScriptGlobal<T>),
                options: ScriptOptions.Default.AddReferences(
                    typeof(System.Linq.Enumerable).Assembly,
                    typeof(Components.CatchGenerator).Assembly
                ));
        }
        
        private static async Task<string> _formatBlock<T>(string code, T global)
        {
            var res = new StringBuilder(code);
            var offset = 0;

            foreach(Match comment in Regex.Matches(code, _commentPattern))
            {
                var commentRes = new List<string>();
                foreach (Match func in Regex.Matches(comment.Groups[1].Value, _funcPattern))
                    commentRes.Add(func.Groups[1].Value);
                res.Remove(comment.Index + offset, comment.Length);

                string formatedCode = await _compileFuncs(commentRes, global);
                res.Insert(comment.Index + offset, formatedCode);
                offset += formatedCode.Length - comment.Length;
            }
            return res.ToString();
        }

        private static async Task<string> _formatCode<T>(string code, T global)
        {
            var res = new StringBuilder(code);
            var offset = 0;

            var matches = Regex.Matches(code, _blockPattern);
            if (matches.Count == 0)
                return await _formatBlock(code, global);

            foreach (Match block in matches)
            {
                res.Remove(block.Index + offset, block.Length);
                var formatedCode = await _formatBlock(block.Groups[1].Value, global);
                res.Insert(block.Index + offset, formatedCode);

                offset += formatedCode.Length - block.Length;
            }
            return res.ToString();
        }

        public static async Task<string> GenerateFromFile<T>(string filePath, T global)
        {
            try
            {
                return await _formatCode(File.ReadAllText(filePath), global);
            }catch(Exception ex)
            {
                throw new CodeGeneratorException(ex.Message);
            }
        }
    }
}
