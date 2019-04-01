using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Uninstaller
{
    public class UninstallException : Exception
    {
        public UninstallException(string msg) : base(msg)
        {

        }
    }
}
