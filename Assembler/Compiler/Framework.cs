using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Assembler.Compiler
{
    public static class Framework
    {
        public static bool Check(string ver)
        {
            switch (ver)
            {
                case "3.5":
                case "4.0":
                case "4.5":
                case "4.5.1":
                case "4.5.2":
                case "4.6":
                case "4.6.1":
                case "4.6.2":
                case "4.7":
                case "4.7.1":
                case "4.7.2":
                    return true;
                default:
                    return false;

            }
        }

        public static bool Exists(string ver)
        {
            var folder = $@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v{ver}\";
            return Directory.Exists(folder);
        }
    }
}
