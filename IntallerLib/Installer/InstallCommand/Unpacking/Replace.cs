using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Unpacking
{
    public class Replace : IInstallCommand
    {
        private string _path;
        private IEnumerable<string> _deletedFiles;
        private byte[] _archive;
        private Func<bool> _stop;

        private IInstallCommand _unpackCommand;

        public event EventHandler<InstallProgressEventArgs> InstallProgressEventHandler;

        public Replace(string path, IEnumerable<string> deletedFiles, byte[] archive, Func<bool> stop)
        {
            _path = path;
            _deletedFiles = deletedFiles;
            _archive = archive;

            _stop = stop;

            _unpackCommand = new Unpack(path, archive, _stop);

            _unpackCommand.InstallProgressEventHandler += (o, e) =>
            {
                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(e.Message,  e.Progress * 0.8));
            };
        }

        public void Do()
        {
            try
            {
                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(Properties.Resources.ReplaceDescription, _path), 100));
                _unpackCommand.Do();
                foreach (var file in _deletedFiles)
                {
                    if (_stop != null && _stop())
                        break;

                    var fullPath = _path + file;
                    if (File.Exists(fullPath))
                    {
                        InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(Properties.Resources.DeleteFile, file), 80.0 + (20.0 / _deletedFiles.Count())));
                        File.Delete(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InstallException(String.Format(Properties.Resources.ReplaceException, _path, ex.Message));
            }
        }

        public void Finish()
        {
            _unpackCommand.Finish();
        }

        public void Undo()
        {
            _unpackCommand.Undo();
        }
    }
}