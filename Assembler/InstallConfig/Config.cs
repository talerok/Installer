using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.InstallConfig
{
    class Config
    {
        public string OutputPath { get; set; }
        public string FrameworkVer { get; set; }
        public string Type { get; set; }

        public string AppName { get; set; }
        public bool UseDefaultPath { get; set; }
        public string DefaultPath { get; set; }
        public string RegistryVersionPath { get; set; }
        public string Version { get; set; }

        public string MinVersion { get; set; }
        public string MaxVersion { get; set; }

        public List<CommandConfig> Commands { get; set; }
        public List<string> StartAfterInstall { get; set; }
    }
}
