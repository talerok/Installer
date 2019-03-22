using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Registry
{
    public class SetPath : IInstallCommand
    {
        public string Description
        {
            get
            {
                return String.Format(InstallerLib.Properties.Resources.SetPathDescription, _appPath);
            }
        }

        private string _regPath;
        private string _appPath;
        private SimpleRegisterCommand _regCommand;

        public SetPath(string appPath, string regPath)
        {
            _regPath = regPath;
            _appPath = appPath;
            _regCommand = new SimpleRegisterCommand(_regPath, "Path", _appPath, RegValueKind.Sz);
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
