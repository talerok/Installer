using Common;
using InstallerLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.FilesBackup
{
    class FilesBackup
    {
        private IEnumerable<string> _files;

        private IDictionary<string, byte[]> _backup = new Dictionary<string, byte[]>();

        public FilesBackup(IEnumerable<string> files)
        {
            _files = files;
        }

        public void Do()
        {
            foreach (var file in _files)
                if (File.Exists(file))
                    _backup.Add(file, File.ReadAllBytes(file));
        }

        public void Finish()
        {
            _backup.Clear();
        }

        private void _restoreFile(byte[] backupFile, string originalFile)
        {
            var dirPath = Path.GetDirectoryName(originalFile);
            if (Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            if (File.Exists(originalFile))
                File.Delete(originalFile);

            File.WriteAllBytes(originalFile, backupFile);
        }

        public void Restore()
        {
            foreach(var file in _files)
                try // Востанавливаем все что можем
                {
                    if (File.Exists(file))
                    {
                        if (_backup.ContainsKey(file)) // Файл был изменен
                        {
                            if (!FilesComparer.Compare(_backup[file], File.ReadAllBytes(file)))
                                _restoreFile(_backup[file], file);
                        }
                        else // Файл был создан
                        {
                            File.Delete(file);
                        }
                    }
                    else 
                    {
                        if (_backup.ContainsKey(file)) // Файл был удален
                            _restoreFile(_backup[file], file);
                    }
                }
                catch
                {

                }
        }
    }
}
