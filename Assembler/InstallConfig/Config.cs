using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.InstallConfig
{
    class Config
    {
        public string AppName { get; set; }
        public bool UseDefaultPath { get; set; }
        public string DefaultPath { get; set; }
        public string RegistryVersionPath { get; set; }
        public string Version { get; set; }
        public string ForVersion { get; set; }
        public List<CommandConfig> Commands { get; set; }
    }
}
