using InstallerLib.Helpers;
using InstallerLib.Installer.InstallInfo.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static InstallerLib.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallInfo
{
    public class GetPath : IInstallInfo<string>
    {
        private string _appName;

        public GetPath(string appName)
        {
            _appName = appName;
        }

        public string GetInfo()
        {
            var config = new ConfigFileManager(_appName).Read();
            if (config == null)
                return null;

            return config.Path;
        }
    }
}
