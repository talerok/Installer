using InstallerLib.Progress;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Interfaces
{

    public interface IInstallCommand
    {
        event EventHandler<ProgressEventArgs> InstallProgressEventHandler;
        void Do();
        void Undo();
        void Finish();
    }
}
