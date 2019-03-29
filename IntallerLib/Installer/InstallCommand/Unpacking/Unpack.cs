using InstallerLib.Installer.InstallCommand.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;
using Ionic.Zip;
using System.Linq;
using System.Security.Cryptography;

namespace InstallerLib.Installer.InstallCommand.Unpacking
{
    public class Unpack : IInstallCommand
    {
        private string _path;
        private string _backupPath = null;
        private byte[] _data;

        private Func<bool> _stop;

        private bool _backupCreated = false;
        private bool _folderExisted = false;
        private string _prevFile = null;

        public Unpack(string path, byte[] data, Func<bool> stop)
        {
            _path = path;
            _data = data;
            _stop = stop;
        }

        public event EventHandler<InstallProgressEventArgs> InstallProgressEventHandler;

        private string _generateDataExceptionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerDataExceptionMessage);
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
            return String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerBundleExtractExceptionMessage, _path);
        }

        private void _copyToDir(string sourceDir, string targetDir)
        {
            System.IO.Directory.CreateDirectory(targetDir);

            foreach (var file in System.IO.Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in System.IO.Directory.GetDirectories(sourceDir))
                _copyToDir(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

        public void Do()
        {    
            try
            {
                _backupPath = $"{_path}-backup-{new Random().Next()}";

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs(String.Format(InstallerLib.Properties.Resources.ZipBundleUnpackerDesription, _data.Length, _path), 0));

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Создание резервной копии", 0));

                if (System.IO.Directory.Exists(_path))
                {
                    _folderExisted = true;
                    _copyToDir(_path, _backupPath);
                    File.SetAttributes(_backupPath, System.IO.FileAttributes.Hidden);
                    _backupCreated = true;
                }
                else
                    System.IO.Directory.CreateDirectory(_path);

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Резервная копия создана", 50));

                using (var stream = new MemoryStream(_data))
                using (var archive = ZipFile.Read(stream))
                {
                    archive.ExtractProgress += (o, e) =>
                    {
                        if (_stop != null && _stop())
                            e.Cancel = true;

                        if (e.CurrentEntry != null && _prevFile != e.CurrentEntry.FileName)
                        {
                            _prevFile = e.CurrentEntry.FileName;

                            InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs($"Распаковка: {e.CurrentEntry.FileName}", 50.0 + 50.0 * e.EntriesExtracted / e.EntriesTotal));
                        }
                    };
                    archive.ExtractAll(_path, ExtractExistingFileAction.OverwriteSilently);
                }

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Архив разархивирован", 100));
            }
            catch (Exception ex)
            {
                throw new InstallException(_generateBundleExtractExceptionMessage() + $" ({ex.Message})");
            }
        }

        public void Finish()
        {
            if (System.IO.Directory.Exists(_backupPath))
                System.IO.Directory.Delete(_backupPath, true);
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

        private void _restoreBackup()
        {
            var backupFiles = System.IO.Directory.GetFiles(_backupPath, "*", SearchOption.AllDirectories).Select(x => x.Replace(_backupPath, ""));

            var progress = 100.0;
            var progressStep = 100.0 / backupFiles.Count();

            foreach(var file in backupFiles)
            {
                progress -= progressStep;
                try // Востанавливаем все что можем
                {
                    var fullpath = _path + file;
                    var backupPath = _backupPath + file;
                    if (!File.Exists(fullpath))
                        File.Copy(_backupPath + file, fullpath);
                    else if(_checkMD5(fullpath) != _checkMD5(fullpath))
                    {
                        File.Delete(fullpath);
                        File.WriteAllBytes(fullpath, File.ReadAllBytes(backupPath));
                    }

                    InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs($"Файл востановлен: {file}", progress));
                }
                catch
                {
                    InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs($"Не удалось востановить файл: {file}", progress));
                }
            }

        }

        public void Undo()
        {
            try
            {
                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Востановление резервной копии", 100));

                if (_folderExisted)
                {
                    if (_backupCreated)
                        _restoreBackup();
                   
                }
                else 
                {
                    if (System.IO.Directory.Exists(_path))
                        System.IO.Directory.Delete(_path, true);
                }

                if (System.IO.Directory.Exists(_backupPath))
                    System.IO.Directory.Delete(_backupPath, true);

                InstallProgressEventHandler.Invoke(this, new InstallProgressEventArgs("Резервная копия востановлена", 0));
            }
            catch (Exception ex)
            {
                throw new InstallException(string.Format(Properties.Resources.ZipBundleUnpackerUndoException, ex.Message)); 
            }
        }
    }
}
