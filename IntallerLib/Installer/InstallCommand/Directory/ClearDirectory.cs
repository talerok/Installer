using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Directory
{
    public class ClearDirectory : IInstallCommand
    {
        public string Description
        {
            get
            {
                return string.Format(Properties.Resources.ClearDirectoryDescription, _path);
            }
        }

        private string _path;
        private string _tempPath;

        public ClearDirectory(string path)
        {
            _path = path;
            _tempPath = $"{path}-backup-{DateTime.Now.ToString().Replace(":", "")}";
        }

        public void Do()
        {
            try
            {
                System.IO.Directory.Move(_path, _tempPath);
                System.IO.Directory.CreateDirectory(_path);
            }
            catch (Exception ex)
            {
                new InstallException(string.Format(Properties.Resources.ClearDirectoryException, _path));
            }
        }

        public void Undo()
        {
            if (System.IO.Directory.Exists(_tempPath)) {
                System.IO.Directory.Delete(_path);
                System.IO.Directory.Move(_tempPath, _path);
            }
        }

        public void Finish()
        {
            if (System.IO.Directory.Exists(_tempPath))
                System.IO.Directory.Delete(_tempPath);
        }
    }
}
