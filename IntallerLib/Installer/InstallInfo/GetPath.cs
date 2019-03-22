using InstallerLib.Installer.Helpers;
using InstallerLib.Installer.InstallInfo.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InstallerLib.Installer.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallInfo
{
    public class GetPath : IInstallInfo<string>
    {
        private string _path;

        public GetPath(string path)
        {
            _path = path;
        }

        public string GetInfo()
        {
            RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
            using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(_path, false))
                if (registry == null)
                    return null;
                else
                    return registry.GetValue("Path").ToString();
        }
    }
}
