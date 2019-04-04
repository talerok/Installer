using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Common;

namespace Assembler.InstallConfig
{
    [Serializable]
    public class ShortCutConfig
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
    }

    [Serializable]
    public class ShortCutsConfig
    {
        public List<ShortCutConfig> DesktopShortCuts { get; set; }
        public List<ShortCutConfig> StartMenuShortCuts { get; set; }
        public List<ShortCutConfig> AutoStart { get; set; }
    }

    [Serializable]
    public class MajorConfig
    {
        public string FileNameTemplate { get; set; }
        public bool UseDefaultPath { get; set; }
        public string DefaultPath { get; set; }
        public ShortCutsConfig ShortCutsConfig { get; set; }
    }

    [Serializable]
    public class MinorConfig
    {
        public string FileNameTemplate { get; set; }
        public string ForVersion { get; set; }
    }

    [Serializable]
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
            var formater = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formater.Serialize(stream, this);
                stream.Position = 0;
                return formater.Deserialize(stream);
            }
        }
    }
}
