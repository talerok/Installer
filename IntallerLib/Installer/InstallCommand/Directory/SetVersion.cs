using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Directory
{
    public class SetVersion : IInstallCommand
    {
        public event EventHandler<InstallProgressEventArgs> InstallProgressEventHandler;

        private string _path;
        private string _version;

        private byte[] _backup;
        private FileAttributes _backupAtributes;

        private string _getFullPath()
        {
            return $@"{_path}\app.version";
        }

        public SetVersion(string version, string path)
        {
            _path = path;
            _version = version;
        }

        public void Do()
        {
            try
            {
                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(InstallerLib.Properties.Resources.SetVersionDescription, _version), 100));
                var fullPath = _getFullPath();
                if (File.Exists(fullPath))
                {
                    _backup = File.ReadAllBytes(fullPath);
                    _backupAtributes = File.GetAttributes(fullPath);
                    File.Delete(fullPath);
                }

                File.WriteAllText(fullPath, _version);
                File.SetAttributes(fullPath, System.IO.FileAttributes.Hidden);
            }
            catch
            {
                throw new InstallException(String.Format(Properties.Resources.SetVersionException, _version));
            }
        }

        public void Undo()
        {
            InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(Properties.Resources.SetVersionUndo, 0));

            var fullPath = _getFullPath();

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            if (_backup != null)
            {
                File.WriteAllBytes(fullPath, _backup);
                File.SetAttributes(fullPath, _backupAtributes);
            }
        }

        public void Finish()
        {
            _backup = null;
        }
    }
}
