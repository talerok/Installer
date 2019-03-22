using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Registry
{
    public class SetVersion : IInstallCommand
    {
        public string Description
        {
            get
            {
                return String.Format(InstallerLib.Properties.Resources.SetVersionDescription, _version);
            }
        }

        private string _path;
        private string _version;
        private SimpleRegisterCommand _regCommand;

        public SetVersion(string verion, string path)
        {
            _version = verion;
            _path = path;
            _regCommand = new SimpleRegisterCommand(_path, "Version", _version, RegValueKind.Sz);
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
