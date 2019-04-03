using InstallerLib.Helpers;
using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Progress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.ShortCut
{
    public class ShortCutsCommand : IInstallCommand
    {
        private Dictionary<string, byte[]> _backup = new Dictionary<string, byte[]>();

        private IEnumerable<ShortCutInfo> _shortCuts;

        private ConfigFile _configBackup;

        private string _path;
        private string _appName;

        public event EventHandler<ProgressEventArgs> InstallProgressEventHandler;

        public ShortCutsCommand(string appName, IEnumerable<ShortCutInfo> shortCuts, string path)
        {
            _shortCuts = shortCuts;
            _path = path;
            _appName = appName;
        }

        private string _getShortCutPath(ShortCutInfo shortCut)
        {
            return $@"{_path}\{shortCut.Name}.lnk";
        }

        public void Do()
        {
            var configReader = new ConfigFileManager(_appName);

            _configBackup = configReader.Read();

            if (_configBackup == null)
                throw new InstallException(Properties.Resources.ConfigFileNotFound);

            double progress = 0;
            double progressStep = 100.0 / _shortCuts.Count();

            //backup
            foreach(var shortCut in _shortCuts)
            {
                var path = _getShortCutPath(shortCut);
                if (File.Exists(path))
                    _backup.Add(path, File.ReadAllBytes(path));
            }

            foreach (var shortCut in _shortCuts)
            {
                var path = _getShortCutPath(shortCut);
                try
                {
                    progress += progressStep;
                    InstallProgressEventHandler.Invoke(this, new ProgressEventArgs(String.Format(Properties.Resources.ShortLinkDescription, path), progress));

                    if (File.Exists(path))
                        File.Delete(path);

                    var folderPath = Path.GetDirectoryName(path);
                    if (!System.IO.Directory.Exists(folderPath))
                        System.IO.Directory.CreateDirectory(folderPath);

                    Helpers.ShortCut.Create(shortCut.FilePath, path, shortCut.Arguments, "");
                }
                catch
                {
                    throw new InstallException(String.Format(Properties.Resources.ShortLinkException, path));
                }
            }

            try
            {
                var newShortCuts = _shortCuts.Select(x => _getShortCutPath(x)).Where(x => !_configBackup.ShortCuts.Contains(x));

                if (!newShortCuts.Any())
                    return;

                var newConfig = _configBackup.Copy();
                newConfig.ShortCuts.AddRange(newShortCuts);
                configReader.Write(newConfig);
            }
            catch
            {
                throw new InstallException(Properties.Resources.ConfigWriteException);
            }

        }

        public void Finish()
        {
            _backup.Clear();
            _configBackup = null;
        }

        public void Undo()
        {
            double progress = 100;
            double progressStep = 100.0 / _shortCuts.Count();

            foreach (var shortCut in _shortCuts)
            {
                var path = _getShortCutPath(shortCut);
                progress -= progressStep;
                InstallProgressEventHandler.Invoke(this, new ProgressEventArgs(String.Format(Properties.Resources.ShortCutUndo, path), progress));
                try // Востанавливаем все что можем
                {
                    File.Delete(path);
                    if (_backup.ContainsKey(path))
                        File.WriteAllBytes(path, _backup[path]);
                }
                catch
                {

                }
            }

            try // Востанавливаем файл конфигурации
            {
                var configReader = new ConfigFileManager(_appName);

                if (_configBackup != null)
                    configReader.Write(_configBackup);
            }
            finally
            {

            }

        }
    }
}
