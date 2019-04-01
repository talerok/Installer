using InstallerLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InstallerLib.FilesBackup
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

        private void _restoreFile(string backupFile, string originalFile)
        {
            var dirPath = Path.GetDirectoryName(originalFile);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.Copy(backupFile, originalFile, true);
        }

        private void _restoreFiles()
        {
            if (!_hasBackup)
                return;

            var info = FoldersSubstitute.Substitute(_backupPath, _originalPath);

            foreach (var file in info)
            {
                try // Востанавливаем все, что можем
                {
                    switch (file.Status)
                    {
                        case FileStatus.Added:
                            File.Delete(_originalPath + file.Path);
                            break;
                        case FileStatus.Deleted:
                        case FileStatus.Modified:
                            _restoreFile(_backupPath + file.Path, _originalPath + file.Path);
                            break;
                    }
                }
                catch
                {

                }
            }
        }

        private void _restoreFolders()
        {
            var folders = Directory.GetDirectories(_originalPath, "*", SearchOption.AllDirectories);
            foreach (var folder in folders)
                try
                {
                    var backupPath = folder.Replace(_originalPath, _backupPath);
                    if(!Directory.Exists(backupPath))
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
            _restoreFiles();
            _restoreFolders();
            _hasBackup = false;

            Finish();
        }

    }
}
