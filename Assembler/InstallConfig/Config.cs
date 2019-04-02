using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.InstallConfig
{
    public class ShortCutConfig
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
    }

    public class Config : ICloneable
    {
        public string OutputPath { get; set; }
        public string IconPath { get; set; }

        public string FrameworkVer { get; set; }
        public string Type { get; set; }

        public string AppName { get; set; }
        public string CompanyName { get; set; }

        public string UninstallerPath { get; set; }

        public string Description { get; set; }
        public string AfterInstallMessage { get; set; }

        public bool UseDefaultPath { get; set; }
        public string DefaultPath { get; set; }
        public string Version { get; set; }
        public string ForVersion { get; set; }

        public string BuildPath { get; set; }

        public string VersionsFolderPath { get; set; }

        public List<string> StartAfterInstall { get; set; }

        public List<ShortCutConfig> DesktopShortCuts { get; set; }
        public List<ShortCutConfig> StartMenuShortCuts { get; set; }
        public List<ShortCutConfig> AutoStart { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
