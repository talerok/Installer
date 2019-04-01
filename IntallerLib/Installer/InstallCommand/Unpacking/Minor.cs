using InstallerLib.Installer.InstallBackup;
using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Unpacking
{
    public class Minor : IInstallCommand
    {
        private string _path;
        private IEnumerable<string> _deletedFiles;
        private byte[] _archive;
        private Func<bool> _stop;

        private PatchBackup _backup;

        public event EventHandler<InstallProgressEventArgs> InstallProgressEventHandler;

        public Minor(string path, IEnumerable<string> deletedFiles, byte[] archive, Func<bool> stop)
        {
            _path = path;
            _deletedFiles = deletedFiles;
            _archive = archive;
            _stop = stop;
        }

        private void _deleteFiles(double unpackMaxProgrs)
        {
            var progress = unpackMaxProgrs;
            var progressOffset = (100.0 - unpackMaxProgrs) / _deletedFiles.Count();

            foreach (var file in _deletedFiles)
            {
                if (_stop != null && _stop())
                    break;

                var fullPath = _path + file;
                if (File.Exists(fullPath))
                {
                    progress += progressOffset;
                    InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(Properties.Resources.DeleteFile, file), progress));
                    File.Delete(fullPath);
                }
            }
        }

        public void Do()
        {
            try
            {
                var archiveFiles = Unpacking.GetFiles(_archive).ToList();

                var backupFiles = new List<string>();
                backupFiles.AddRange(archiveFiles);
                backupFiles.AddRange(_deletedFiles);

                _backup = new PatchBackup(_path, backupFiles);

                var unpackMaxProgrs = 50 + 50.0 * (archiveFiles.Count() / backupFiles.Count());

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerDesription, _archive.Length, _path), 0));

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Создание резервной копии", 0));

                _backup.Do();

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Резервная копия создана", 50));

                Unpacking.Unpack(_path, _archive, _stop, InstallProgressEventHandler, this, 50, unpackMaxProgrs);

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Архив разархивирован", unpackMaxProgrs));

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(Properties.Resources.ReplaceDescription, _path), unpackMaxProgrs));

                _deleteFiles(unpackMaxProgrs);
            }
            catch (Exception ex)
            {
                throw new InstallException(String.Format(Properties.Resources.ReplaceException, _path, ex.Message));
            }
        }

        public void Finish()
        {
            _backup.Finish();
            _backup = null;
        }

        public void Undo()
        {
            try
            {
                if (_backup == null)
                    return;

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Востановление резервной копии", 100));

                _backup.Restore();

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Резервная копия востановлена", 0));
            }
            catch (Exception ex)
            {
                throw new InstallException(string.Format(Properties.Resources.ZipBundleUnpackerUndoException, ex.Message));
            }
        }
    }
}