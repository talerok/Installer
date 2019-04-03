using InstallerLib.Installer.InstallCommand.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.ShortCut
{
    public class StartMenu : ShortCutsCommand
    {
        public StartMenu(string appName, IEnumerable<ShortCutInfo> shortCuts) : base(appName, shortCuts, _getStartMenuPath())
        {

        }

        private static string _getStartMenuPath()
        {
            RegistryKey key = Microsoft.Win32.Registry.LocalMachine;
            key = key.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders");
            return key.GetValue("Common Start Menu").ToString();
        }

    }
}
