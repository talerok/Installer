using System;
using System.Collections.Generic;
using System.Text;

namespace InstallerLib.Installer.InstallCommand
{
    public class InstallException : Exception
    {
        public InstallException(string msg) : base(msg) { }
    }
}
