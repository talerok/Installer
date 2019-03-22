using System;
using System.Collections.Generic;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Interfaces
{
    public interface IInstallCommand
    {
        string Description { get; }
        void Do();
        void Undo();
        void Finish();
    }
}
