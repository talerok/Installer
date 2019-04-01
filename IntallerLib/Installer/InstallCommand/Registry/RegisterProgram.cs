using InstallerLib.Helpers;
using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Progress;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static InstallerLib.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallCommand.Registry
{
    public class RegisterProgram : IInstallCommand
    {

        private string _appName;
        private string _version;
        private string _unstallPath;
        private string _companyName;

        private IEnumerable<RegValueInfo> _backup;

        public event EventHandler<ProgressEventArgs> InstallProgressEventHandler;

        public RegisterProgram(string appName, string appVersion, string companyName, string unstallPath)
        {
            _appName = appName;
            _version = appVersion;
            _companyName = companyName;
            _unstallPath = unstallPath;
        }

        public void Do()
        {
            try
            {
                RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
                using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
                {
                    InstallProgressEventHandler.Invoke(this, new ProgressEventArgs("Регистрация программы", 100));

                    RegistryKey key = registry.OpenSubKey(_appName, true);

                    if (key != null)
                    {
                        _backup = RegValueInfo.Get(key);
                        key.Clear();
                    }
                    else
                    {
                        registry.CreateSubKey(_appName);
                        key = registry.OpenSubKey(_appName, true);
                    }

                    key.SetValue("DisplayName", _appName, RegistryValueKind.String);
                    key.SetValue("ApplicationVersion", _version, RegistryValueKind.String);
                    key.SetValue("Publisher", _companyName, RegistryValueKind.String);
                    key.SetValue("DisplayIcon", _unstallPath, RegistryValueKind.String);
                    key.SetValue("DisplayVersion", _version, RegistryValueKind.String);
                    key.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"), RegistryValueKind.String);
                    key.SetValue("UninstallString", _unstallPath, RegistryValueKind.String);

                }
            }
            catch (Exception ex)
            {
                throw new InstallException($"Ошибка регистрации программы: {ex.Message}");
            }
        }

        public void Finish()
        {
            _backup = null;
        }

        public void Undo()
        {
            RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
            using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                InstallProgressEventHandler.Invoke(this, new ProgressEventArgs("Откат регистрации программы", 0));

                RegistryKey key = registry.OpenSubKey(_appName);
                if (key == null || _backup == null)
                    return;

                key.Clear();
                RegValueInfo.Set(_backup, key);
            }
        }
    }
}
