using InstallerLib.Helpers;
using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Progress;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Directory
{
    public class SetPath : IInstallCommand
    {
        public event EventHandler<ProgressEventArgs> InstallProgressEventHandler;

        private string _appName;
        private string _appPath;

        private ConfigFile _backup;

        public SetPath(string appName, string path)
        {
            _appName = appName;
            _appPath = path;

        }

        public void Do()
        {
            try
            {
                InstallProgressEventHandler.Invoke(this, new ProgressEventArgs(String.Format(Properties.Resources.SetPathDescription, _appPath), 100));

                var configManager = new ConfigFileManager(_appName);
                _backup = configManager.Read();
                ConfigFile config;
                if(_backup == null)
                {
                    config = new ConfigFile
                    {
                        Path = _appPath,
                        ShortCuts = new List<string>()
                    };
                }
                else
                {
                    config = _backup.Copy();
                    config.Path = _appPath;
                }
                configManager.Write(config);
            }
            catch
            {
                new InstallException(String.Format(Properties.Resources.SetPathException, _appPath));
            }
        }

        public void Undo()
        {
            try
            {
                var configManager = new ConfigFileManager(_appName);
                if (_backup == null)
                    configManager.Delete();
                else
                    configManager.Write(_backup);

            }
            catch
            {

            }
        }

        public void Finish()
        {
            _backup = null;
        }
    }
}
