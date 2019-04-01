using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InstallerLib.Progress
{
    public class ProgressEventArgs : EventArgs
    {
        public double Progress { get; }
        public string Message { get; }

        public ProgressEventArgs(string message, double progress)
        {
            Message = message;
            Progress = progress;
        }
    }
}
