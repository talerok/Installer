using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.ShortCut
{
    public class ShortCutInfo
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Arguments { get; set; }

        public ShortCutInfo(string name, string filePath, string arguments = "")
        {
            Name = name;
            FilePath = filePath;
            Arguments = arguments;
        }
    }
}
