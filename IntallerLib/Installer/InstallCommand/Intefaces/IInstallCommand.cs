using System;
using System.Collections.Generic;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Interfaces
{

    public class InstallProgressEventArgs : EventArgs
    {
        public double Progress { get; }
        public string Message { get; }

        public InstallProgressEventArgs(string message, double progress)
        {
            Message = message;
            Progress = progress;
        }
    }

    public interface IInstallCommand
    {
        event EventHandler<InstallProgressEventArgs> InstallProgressEventHandler;
        void Do();
        void Undo();
        void Finish();
    }
}
