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
        private string _bundleName;
        private string _path;
        private IEnumerable<string> _deletedFiles;
        private byte[] _archive;

        private IInstallCommand _unpackCommand;

        public Replace(string bundleName, string path, IEnumerable<string> deletedFiles, byte[] archive)
        {
            _bundleName = bundleName;
            _path = path;
            _deletedFiles = deletedFiles;
            _archive = archive;

            _unpackCommand = new ZipBundleUnpacker(bundleName, path, archive);
        }

        public string Description => String.Format(Properties.Resources.ReplaceDescription, _path, _bundleName);

        public void Do()
        {
            try
            {
                _unpackCommand.Do();
                foreach (var file in _deletedFiles)
                {
                    var fullPath = _path + file;
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                throw new InstallException(String.Format(Properties.Resources.ReplaceException, _path, _bundleName, ex.Message));
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