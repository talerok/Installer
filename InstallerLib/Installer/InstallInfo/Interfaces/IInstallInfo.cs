using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallInfo.Interfaces
{
    interface IInstallInfo<T>
    {
        T GetInfo();
    }
}
