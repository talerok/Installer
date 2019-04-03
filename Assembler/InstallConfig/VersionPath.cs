using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.InstallConfig
{
    static class VersionPath
    {
        public static string Generate(string appName, string version)
        {
            return $@"Versions\{appName}\{version}";
        }
    }
}
