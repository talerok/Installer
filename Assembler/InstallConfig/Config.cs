using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace Assembler.InstallConfig
{
    public class ShortCutConfig
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
    }

    public class ShortCutsConfig
    {
        public List<ShortCutConfig> DesktopShortCuts { get; set; }
        public List<ShortCutConfig> StartMenuShortCuts { get; set; }
        public List<ShortCutConfig> AutoStart { get; set; }
    }

    public class MajorConfig
    {
        public string FileNameTemplate { get; set; }
        public bool UseDefaultPath { get; set; }
        public string DefaultPath { get; set; }
        public ShortCutsConfig ShortCutsConfig { get; set; }
    }

    public class MinorConfig
    {
        public string FileNameTemplate { get; set; }
        public string ForVersion { get; set; }
    }

    public class Config : ICloneable
    {
        public string IconPath { get; set; }
        public string FrameworkVer { get; set; }

        public string AppName { get; set; }
        public string CompanyName { get; set; }
        public string Version { get; set; }

        public string Description { get; set; }
        public string AfterInstallMessage { get; set; }

        public MajorConfig MajorConfig { get; set; }
        public MinorConfig MinorConfig { get; set; }

        public string BuildPath { get; set; }

        public List<string> StartAfterInstall { get; set; }

        public object Clone()
        {
            return this.CloneObject();
        }
    }
}
