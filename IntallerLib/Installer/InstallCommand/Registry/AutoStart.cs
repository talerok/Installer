using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Installer.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using static InstallerLib.Installer.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallCommand.Registry
{
    public class AutoStart : IInstallCommand
    {
        private string _appName;
        private string _path;
        private string _backupValue = null;
        private SimpleRegisterCommand _regCommand;

        public string Description
        {
            get
            {
                return String.Format(InstallerLib.Properties.Resources.AutoStartCommandDescription, _appName, _path);
            }
        }

        public AutoStart(string appName, string path)
        {
            _appName = appName;
            _path = path;
            _regCommand = new SimpleRegisterCommand(@"\Software\Microsoft\Windows\CurrentVersion\Run", _appName, _path, RegValueKind.Sz);
            
        }

        public void Do()
        {
            _regCommand.Do();
        }

        public void Undo()
        {
            _regCommand.Undo();
        }

        public void Finish()
        {
            _regCommand.Finish();
        }
    }
}
