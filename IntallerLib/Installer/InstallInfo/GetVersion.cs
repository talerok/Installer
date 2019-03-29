using InstallerLib.Installer.Helpers;
using InstallerLib.Installer.InstallInfo.Interfaces;
using Microsoft.Win32;
using System.IO;
using static InstallerLib.Installer.Helpers.RegistryExtensions;

namespace InstallerLib.Installer.InstallInfo
{
    public class GetVersion : IInstallInfo<string>
    {
        private string _appName;

        public GetVersion(string appName)
        {
            _appName = new GetPath(appName).GetInfo();
        }

        private string _getFullPath()
        {
            if (_appName == null)
                return null;
            return $@"{_appName}\app.version";
        }

        public string GetInfo()
        {
            var fullPath = _getFullPath();

            if (fullPath == null)
                return null;

            if (!File.Exists(fullPath))
                return null;

            return File.ReadAllText(fullPath);
        }
    }
}