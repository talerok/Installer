using Assembler.InstallConfig.Interfaces;
using System;
using Newtonsoft.Json;
using System.IO;

namespace Assembler.InstallConfig
{
    public class JSONConfigReader : IConfigReader
    {
        private string _path;

        public JSONConfigReader(string path)
        {
            _path = path;
        }

        private string _readConfig()
        {
            var config = File.ReadAllText(_path);
            return config;
        }

        public Config Read()
        {
            var config = _readConfig();
            return JsonConvert.DeserializeObject<Config>(config);
        }
    }
}
