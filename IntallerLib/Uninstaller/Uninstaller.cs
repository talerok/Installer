using InstallerLib.FilesBackup;
using InstallerLib.Helpers;
using InstallerLib.Progress;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static InstallerLib.Helpers.RegistryExtensions;

namespace InstallerLib.Uninstaller
{
    public class Uninstaller
    {
        private string _appName;
        private FullBackup _folderBackup = null;
        private FilesBackup.FilesBackup _shortCutsBackup = null;
        private ConfigFile _configBackup = null;
        private IEnumerable<RegValueInfo> _regBackup = null;

        public event EventHandler<ProgressEventArgs> UninstallEventHandler;

        public Uninstaller(string appName)
        {
            _appName = appName;
        }

        public bool Check()
        {
            var configManager = new ConfigFileManager(_appName);
            return configManager.ConfigFileExists();
        }

        private IEnumerable<RegValueInfo> _getRegBackup()
        {
            RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
            using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                RegistryKey key = registry.OpenSubKey(_appName, true);
                if (key == null)
                    return null;
                else
                    return RegValueInfo.Get(key);
            }
        }

        private void _restoreRegBackup()
        {
            if (_regBackup == null)
                return;
            RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
            using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                RegistryKey key = registry.OpenSubKey(_appName, true);
                if (key != null)
                {
                    key.Clear();
                }
                else
                {
                    registry.CreateSubKey(_appName);
                    key = registry.OpenSubKey(_appName);
                }
                RegValueInfo.Set(_regBackup, key);
            }
        }

        private void _deleteRegInfo()
        {
            RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
            using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                RegistryKey key = registry.OpenSubKey(_appName, true);
                if (key != null)
                    registry.DeleteSubKey(_appName);
            }
        }

        public string GetUnstallPath()
        {
            var configManager = new ConfigFileManager(_appName);
            var config = configManager.Read();
            return config != null ? config.Path : null;
        }

        public void Do()
        {
            var configManager = new ConfigFileManager(_appName);
            var config = configManager.Read();
            if (config == null)
                throw new UninstallException("Файл конфигурации не найден");

            try
            {

                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Создание резервной копии", 0));
                
                if (Directory.Exists(config.Path)){
                    _folderBackup = new FullBackup(config.Path);
                    _folderBackup.Do();
                }

                _regBackup = _getRegBackup();

                _shortCutsBackup = new FilesBackup.FilesBackup(config.ShortCuts);
                _shortCutsBackup.Do();
                
                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Резервная копия создана", 50));

                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Удаление программы", 50));
                if(Directory.Exists(config.Path))
                    Directory.Delete(config.Path, true);
                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Удаление ярлыков", 85));
                foreach (var shortCut in config.ShortCuts)
                {
                    if(File.Exists(shortCut))
                        File.Delete(shortCut);
                }
                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Удаление файла конфигурации", 90));
                _configBackup = config;
                configManager.Delete();
                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Удаление регистрации программы", 95));
                _deleteRegInfo();
                UninstallEventHandler.Invoke(this, new ProgressEventArgs("Программа удалена", 100));
            }
            catch (Exception ex)
            {
                throw new UninstallException($"Ошибка удаления программы: {ex.Message}");
            }
        }

        public void Undo()
        {
            if (_folderBackup != null)
                _folderBackup.Restore();
            if (_shortCutsBackup != null)
                _shortCutsBackup.Restore();
            if(_configBackup != null)
            {
                var configManager = new ConfigFileManager(_appName);
                configManager.Write(_configBackup);
            }
            _restoreRegBackup();
            Finish();
        }

        public void Finish()
        {
            if (_folderBackup != null)
            {
                _folderBackup.Finish();
                _folderBackup = null;
            }
            if (_shortCutsBackup != null)
            {
                _shortCutsBackup.Finish();
                _shortCutsBackup = null;
            }
            if (_configBackup != null)
                _configBackup = null;
            if (_regBackup != null)
                _regBackup = null;
        }
    }
}
