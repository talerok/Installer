using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace InstallerLib.Installer.Helpers
{
    class ConfigFile
    {
        public string Path { get; set; }
        public List<string> ShortCuts { get; set; }

        public ConfigFile Copy()
        {
            return new ConfigFile
            {
                Path = this.Path,
                ShortCuts = new List<string>(ShortCuts)
            };
        }

    }

    class ConfigFileManager
    {
        private string _appName;

        private string _getFullPath()
        {
            return $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\{_appName}.info";
        }

        public ConfigFileManager(string appName) {
            _appName = appName;
        }

        public bool ConfigFileExists()
        {
            var fullPath = _getFullPath();
            return File.Exists(fullPath);
        }

        public ConfigFile Read()
        {
            var fullPath = _getFullPath();

            if (File.Exists(fullPath))
                return JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(fullPath));
            else
                return null;
        }

        public void Write(ConfigFile config)
        {
            var fullPath = _getFullPath();

            if (File.Exists(fullPath))
                File.Delete(fullPath);
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(config));
            File.SetAttributes(fullPath, FileAttributes.Hidden);
        }

        public void Delete()
        {
            var fullPath = _getFullPath();

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }
}
