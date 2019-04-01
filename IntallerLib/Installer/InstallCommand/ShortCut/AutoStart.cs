using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using static InstallerLib.Helpers.RegistryExtensions;
using System.IO;

namespace InstallerLib.Installer.InstallCommand.ShortCut
{
    public class AutoStart : ShortCutsCommand
    {
        public AutoStart(string appName, IEnumerable<ShortCutInfo> shortCuts) : base(appName, shortCuts, _getStartUpPath())
        {

        }
       
        private static string _getStartUpPath()
        {
            RegistryKey key = Microsoft.Win32.Registry.LocalMachine;
            key = key.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders");
            return key.GetValue("Common Startup").ToString();
        }
    }
}
