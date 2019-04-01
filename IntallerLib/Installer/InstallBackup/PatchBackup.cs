using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InstallerLib.Installer.InstallBackup
{
    class PatchBackup
    {
        private IEnumerable<string> _files;
        private string _originalPath;
        private string _backupPath;

        private bool _hasBackup = false;

        private string _checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }

        private string _getOriginalPath(string file)
        {
            return $@"{_originalPath}\{file}"; ;
        }

        private string _getBackupPath(string file)
        {
            return $@"{_backupPath}\{file}"; 
        }

        private string _generateBackupFolderPath()
        {
            return $@"{_originalPath}-{DateTime.Now.ToString().Replace(":","")}";
        }

        public PatchBackup(string folder, IEnumerable<string> files)
        {
            _originalPath = folder;
            _backupPath = _generateBackupFolderPath();
            _files = files;
        } 

        public void Do()
        {
            _hasBackup = false;
            if (Directory.Exists(_backupPath))
                Directory.Delete(_backupPath, true);

            Directory.CreateDirectory(_backupPath);
            File.SetAttributes(_backupPath, System.IO.FileAttributes.Hidden);

            foreach (var file in _files)
            {
                var fullPath = _getOriginalPath(file);
                if (File.Exists(fullPath))
                {
                    var fullBackupPath = _getBackupPath(file);
                    var dirBacupPath = Path.GetDirectoryName(fullBackupPath);
                    if (!Directory.Exists(dirBacupPath))
                        Directory.CreateDirectory(dirBacupPath);
                    File.Copy(fullPath, fullBackupPath);
                }
            }
            _hasBackup = true;
        }

        public void Finish()
        {
            _hasBackup = false;
            if (Directory.Exists(_backupPath))
                Directory.Delete(_backupPath, true);
        }

        public void Restore()
        {
            if (!_hasBackup)
                return;

            foreach (var file in _files)
            {
                try // востанавливаем все что сможем
                {
                    var originalPath = _getOriginalPath(file);
                    var backupPath = _getBackupPath(file);

                    var be = File.Exists(backupPath);
                    var oe = File.Exists(originalPath);

                    if (!be && oe)
                        File.Delete(originalPath);
                    else if (be && oe)
                    {
                        if (_checkMD5(backupPath) != _checkMD5(originalPath))
                            File.Copy(backupPath, originalPath, true);
                    }
                    else if (be && !oe)
                    {
                        File.Copy(backupPath, originalPath, true);
                    }
                }
                catch
                {

                }
            }
            Finish();
        }

    }
}
