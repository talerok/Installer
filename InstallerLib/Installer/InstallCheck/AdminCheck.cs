using InstallerLib.Installer.InstallCheck.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace InstallerLib.Installer.InstallCheck
{
    public class AdminCheck : IInstallCheck
    {
        public bool Check()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
