 using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Installer.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using static InstallerLib.Installer.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallCommand.Registry
{
    public class SimpleRegisterCommand : IInstallCommand
    {

        private string _path;
        private string _value;
        private string _name;
        private RegValueKind _valueKind;

        private object _backupValue;
        private RegistryValueKind _backupRegistryValueKind;

        public string Description
        {
            get
            {
                return String.Format(InstallerLib.Properties.Resources.SimpleRegisterCommandDescription, _value, _valueKind, _path);
            }
        }

        public SimpleRegisterCommand(string path, string name, string value, RegValueKind valueKind)
        {
            _path = path;
            _name = name;
            _value = value;
            _valueKind = valueKind;
        }

        private static object _converValue(string value, RegValueKind valueKind)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            switch (valueKind)
            {
                case RegValueKind.Binary:
                    return Encoding.ASCII.GetBytes(value);
                case RegValueKind.Dword:
                    if (Int32.TryParse(value, out int intRes))
                        return intRes;
                    break;
                case RegValueKind.Qword:
                    if (Int64.TryParse(value, out long longRes))
                        return longRes;
                    break;
                case RegValueKind.MultiSZ:
                    return value.Split('\n');
                case RegValueKind.ExpandSZ:
                case RegValueKind.Sz:
                    return value;
            }
            return null;
        }

        private static RegistryValueKind _convertValueKind(RegValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegValueKind.Binary:
                    return RegistryValueKind.Binary;
                case RegValueKind.Dword:
                    return RegistryValueKind.DWord;
                case RegValueKind.Qword:
                    return RegistryValueKind.QWord;
                case RegValueKind.MultiSZ:
                    return RegistryValueKind.MultiString;
                case RegValueKind.ExpandSZ:
                    return RegistryValueKind.ExpandString;
                case RegValueKind.Sz:
                    return RegistryValueKind.String;
                default:
                    return RegistryValueKind.Unknown;
            }
        }

        private string _generateConvertExceptionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.SimpleRegisterCommandConvertExceptionMessage, _value, _valueKind, _path);
        }

        private string _generateRegisterExcpetionMessage()
        {
            return String.Format(InstallerLib.Properties.Resources.SimpleRegisterCommandRegisterExcpetionMessage, _value, _valueKind, _path);
        }

        public void Do()
        {
            var val = _converValue(_value, _valueKind);
            var keyKind = _convertValueKind(_valueKind);

            if (val == null || keyKind == RegistryValueKind.Unknown)
                throw new InstallException(_generateConvertExceptionMessage());
            try
            {
                RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
                using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType))
                {
                    var curDir = registry;
                    foreach (var dir in _path.Split('\\'))
                    {
                        curDir.CreateSubKey(dir);
                        curDir = curDir.OpenSubKey(dir, true);
                    }

                    _backupValue = curDir.GetValue(_name);
                    if (_backupValue != null)
                    {
                        _backupRegistryValueKind = curDir.GetValueKind(_name);
                        curDir.DeleteValue(_name);
                    }

                    curDir.SetValue(_name, val, keyKind);              
                }
            }
            catch
            {
                throw new InstallException(_generateRegisterExcpetionMessage());
            }
        }

        public void Undo()
        {
            try
            {
                RegistryHiveType registryHiveType = OSInfo.IsOS64Bit() ? RegistryHiveType.X64 : RegistryHiveType.X86;
                using (var registry = OpenBaseKey(RegistryHive.LocalMachine, registryHiveType))
                {
                    var subKey = registry.OpenSubKey(_path);
                    if (subKey == null)
                        return;

                    if (_backupValue == null)
                    {
                        if (subKey.GetValue(_name) != null)
                            subKey.DeleteValue(_name);
                    }
                    else
                    {
                        subKey.SetValue(_name, _backupValue, _backupRegistryValueKind);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InstallException(string.Format(Properties.Resources.SimpleRegisterCommandUndoException, _name, _path, ex.Message));
            }
        }

        public void Finish()
        {
            _backupValue = null;
        }
    }
}
