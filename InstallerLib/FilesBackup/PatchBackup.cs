using Common;
using InstallerLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InstallerLib.FilesBackup
{
    class PatchBackup
    {
        private IEnumerable<string> _files;
        private string _originalPath;
        private string _backupPath;

        private bool _hasBackup = false;

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

        private void _restoreFile(string backupFile, string originalFile)
        {
            var dirPath = Path.GetDirectoryName(originalFile);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.Copy(backupFile, originalFile, true);
        }

        private void _restoreFolders()
        {
            var folders = Directory.GetDirectories(_originalPath, "*", SearchOption.AllDirectories);
            foreach (var folder in folders)
                try
                {
                    var backupPath = folder.Replace(_originalPath, _backupPath);
                    if (!Directory.Exists(backupPath))
                        Directory.Delete(folder, true);
                }
                catch
                {

                }
        }

        private void _restoreFiles()
        {
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

        public void Restore()
        {
            if (!_hasBackup)
                return;

            _restoreFiles();
            _restoreFolders();

            Finish();
        }

    }
}
