using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;
using Ionic.Zip;
using System.Linq;

namespace InstallerLib.Installer.InstallCommand.Unpacking
{
    public class ZipBundleUnpacker : IInstallCommand
    {
        private string _bundleName;
        private string _path;
        private string _tempPath;
        private string _backupPath;
        private byte[] _data;

        private bool _hasBackup = false;

        private static string _createTemporaryFolder(string path, string prefix, string bundleName)
        {
            var tempPath = $@"{path}\{prefix}-{bundleName}-{DateTime.Now.ToString().Replace(":", "")}";
            if (System.IO.Directory.Exists(tempPath))
                System.IO.Directory.Delete(tempPath, true);

            DirectoryInfo di = System.IO.Directory.CreateDirectory(tempPath);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            return tempPath;
        }

        private static IEnumerable<string> _getAllFiles(string path)
        {
            var files = System.IO.Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            return files;
        }

        public ZipBundleUnpacker(string bundleName, string path, byte[] data)
        {
            _bundleName = bundleName;
            _path = path;
            _data = data;
        }

        public string Description
        {
            get
            {
                return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerDesription, _bundleName, _data.Length, _path);
            }
        }

        private string _generateDataExceptionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerDataExceptionMessage, _bundleName);
        }

        private string _generateBundleNameExceptionMessage()
        {
            return InstallerLib.Properties.Resources.ZipBundleUnpackerBundleNameExceptionMessage;
        }

        private string _generateBundlePathExceptionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerBundlePathExceptionMessage, _path);
        }

        private string _generateBundleExtractExceptionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerBundleExtractExceptionMessage, _bundleName, _path);
        }

        private static void _fillBackupDictionary(string path, string tempPath, string backupPath)
        {
            var files = _getAllFiles(tempPath).Select(x => x.Replace(tempPath, ""));
            foreach(var file in files)
            {
                var fileDir = path + file;
                var backupFileDir = backupPath + file;
                if (File.Exists(fileDir))
                {
                    var dir = Path.GetDirectoryName(backupFileDir);
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);

                    File.Copy(fileDir, backupFileDir);
                }    
            }
        }

        private static void _copyFiles(string path, string tempPath)
        {
            var files = _getAllFiles(tempPath).Select(x => x.Replace(tempPath, ""));
            foreach (var file in files)
            {
                var fileDir = path + file;
                var tempFileDir = tempPath + file;

                var dir = Path.GetDirectoryName(fileDir);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                File.Copy(tempFileDir, fileDir, true);
            }
        }

        private static void _restoreFiles(string path, string tempPath, string backupPath)
        {
            var files = _getAllFiles(tempPath).Select(x => x.Replace(tempPath, ""));
            foreach(var file in files)
            {
                var fileDir = path + file;
                var tempFileDir = tempPath + file;
                var backupFileDir = backupPath + file;

                try // Востанавливаем все что можно
                {
                    if (File.Exists(backupFileDir))
                        File.Copy(backupFileDir, fileDir, true);
                    else if (File.Exists(fileDir))
                        File.Delete(fileDir);
                }
                catch { }

            }
        } 

        public void Finish()
        {
            if (System.IO.Directory.Exists(_tempPath))
                System.IO.Directory.Delete(_tempPath, true);

            if (System.IO.Directory.Exists(_backupPath))
                System.IO.Directory.Delete(_backupPath, true);
        }

        public void Do()
        {
            if (String.IsNullOrEmpty(_bundleName))
                throw new InstallException(_generateBundleNameExceptionMessage());
            if(_data == null)
                throw new InstallException(_generateDataExceptionMessage());
            try
            {
                if (!System.IO.Directory.Exists(_path))
                    System.IO.Directory.CreateDirectory(_path);

                _tempPath = _createTemporaryFolder(_path, "temp", _bundleName);
                _backupPath = _createTemporaryFolder(_path, "backup", _bundleName);

                using (var stream = new MemoryStream(_data))
                using (var archive = ZipFile.Read(stream))
                    archive.ExtractAll(_tempPath); // распаковываем во временную папку

                _fillBackupDictionary(_path, _tempPath, _backupPath); // делаем бэкап
                _hasBackup = true; // Флаг того, что бэкап был создан (для Undo), т.к. без бэкапа не востанавить

                _copyFiles(_path, _tempPath); // копируем файлы из временной папки
            }
            catch (Exception ex)
            {
                throw new InstallException(_generateBundleExtractExceptionMessage() + $" ({ex.Message})");
            }
        }

        public void Undo()
        {
            try
            {
                if (_hasBackup && System.IO.Directory.Exists(_tempPath) && System.IO.Directory.Exists(_backupPath) && System.IO.Directory.Exists(_path))
                    _restoreFiles(_path, _tempPath, _backupPath);
                Finish();
            }
            catch (Exception ex)
            {
                throw new InstallException(string.Format(Properties.Resources.ZipBundleUnpackerUndoException, _bundleName, ex.Message)); 
            }
        }

        
    }
}
