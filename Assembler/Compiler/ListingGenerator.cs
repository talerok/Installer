using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.Compiler
{
    static class ListingGenerator
    {
        private const string _tabString = "    ";

        private static int _countCloseBraces(string line)
        {
            int res = 0;
            for (var i = 0; i < line.Length; i++)
                if (line[i] == '}')
                    res++;
                else
                    break;
            return res;
        }

        private static int _countOpenBraces(string line)
        {
            int res = 0;
            for (var i = line.Length - 1; i >= 0; i--)
                if (line[i] == '{')
                    res++;
                else
                    break;
            return res;
        }

        private static string _getTabulation(int level)
        {
            var res = new StringBuilder();
            for (var i = 0; i < level; i++)
                res.Append(_tabString);
            return res.ToString();
        }

        public static string GenerateCodeLisning(string code, bool lineCaptions = true)
        {
            var res = new StringBuilder();
            var lines = code.Split('\n');
            int tabLevel = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) || string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var formLine = lines[i].Replace('\r',' ').Replace('\n', ' ').TrimStart(' ').TrimEnd(' ');
                tabLevel -= _countCloseBraces(formLine);

                if(lineCaptions)
                    res.AppendLine($@"{i + 1}{_tabString}{_getTabulation(tabLevel)}{lines[i].TrimStart(' ').TrimEnd(' ')}");
                else
                    res.Append($@"{_getTabulation(tabLevel)}{lines[i].TrimStart(' ').TrimEnd(' ')}");
                tabLevel += _countOpenBraces(formLine);

            }
            return res.ToString();
        }

    }
}
