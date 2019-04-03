﻿using InstallerLib.Installer.InstallCommand.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.ShortCut
{
    public class Desktop : ShortCutsCommand
    {

        public Desktop(string appName, IEnumerable<ShortCutInfo> shortCuts) : base(appName, shortCuts, _getDesktopPath())
        {

        }

        private static string _getDesktopPath()
        {
            RegistryKey key = Microsoft.Win32.Registry.LocalMachine;
            key = key.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders");
            return key.GetValue("Common Desktop").ToString();
        }

    }
}
