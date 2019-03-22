using InstallerLib.Installer.Helpers;
using InstallerLib.Installer.InstallCheck.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InstallerLib.Installer.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallCheck
{
    public class VersionCheck : IInstallCheck
    {
        private string _version;
        private string _path;

        public VersionCheck(string version, string path)
        {
            _version = version;
            _path = path;
        }

        public bool Check()
        {
            RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
            using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(_path, false))
                if (registry == null)
                    return false;
                else
                    return registry.GetValue("Version").ToString() == _version;
                   
        }
    }
}
