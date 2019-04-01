using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Ionic.Zip;
using InstallerLib.Installer.InstallBackup;

namespace InstallerLib.Installer.InstallCommand.Unpacking
{
    public class Major : IInstallCommand
    {
        private string _path;
        private byte[] _data;

        private Func<bool> _stop;

        private FullBackup _backup;

        public Major(string path, byte[] data, Func<bool> stop)
        {
            _path = path;
            _data = data;
            _stop = stop;
        }

        public event EventHandler<InstallProgressEventArgs> InstallProgressEventHandler;


        private string _generateBundleExtractExceptionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerBundleExtractExceptionMessage, _path);
        }

        public void Do()
        {    
            try
            {
                _backup = new FullBackup(_path);

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerDesription, _data.Length, _path), 0));

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Создание резервной копии", 0));

                _backup.Do();

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Резервная копия создана", 50));

                Unpacking.Unpack(_path, _data, _stop, InstallProgressEventHandler, this, 50.0, 100.0);

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Архив разархивирован", 100));
            }
            catch (Exception ex)
            {
                throw new InstallException(_generateBundleExtractExceptionMessage() + $" ({ex.Message})");
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
