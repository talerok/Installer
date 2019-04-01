using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InstallerLib.Installer.InstallBackup
{
    class FullBackup
    {
        private string _originalPath;
        private string _backupPath;

        private bool _hasBackup = false;

        private void _copyToDir(string sourceDir, string targetDir)
        {
            System.IO.Directory.CreateDirectory(targetDir);

            foreach (var file in System.IO.Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in System.IO.Directory.GetDirectories(sourceDir))
                _copyToDir(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

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

        private string _generateBackupFolderPath()
        {
            return $@"{_originalPath}-{DateTime.Now.ToString().Replace(":", "")}";
        }

        public FullBackup(string path)
        {
            _originalPath = path;
            _backupPath = _generateBackupFolderPath();
        }

        public void Do()
        {
            if (Directory.Exists(_backupPath))
                Directory.Delete(_backupPath, true);

            _hasBackup = false;
            _copyToDir(_originalPath, _backupPath);
            _hasBackup = true;
        }

        private void _restoreFiles()
        {
            foreach (var backupFile in Directory.GetFiles(_backupPath, "*", SearchOption.AllDirectories))
            {
                try // Востанавливаем все, что можем
                {
                    var originalFile = backupFile.Replace(_backupPath, _originalPath);
                    if (!File.Exists(originalFile))
                        File.Delete(originalFile);
                    else if (_checkMD5(originalFile) != _checkMD5(backupFile))
                        File.Copy(backupFile, originalFile, true);
                }
                catch
                {

                }
            }
        }

        private void _restoreFolders()
        {
            var folders = Directory.GetDirectories(_backupPath, "*", SearchOption.AllDirectories).Select(x => x.Replace(_backupPath, _originalPath))
                .Where(x => !Directory.Exists(x));
            foreach (var folder in folders)
                try
                {
                    Directory.Delete(folder, true);
                }
                catch
                {

                }
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
            _restoreFolders();
            _restoreFiles();
            _hasBackup = false;

            Finish();
        }

    }
}
