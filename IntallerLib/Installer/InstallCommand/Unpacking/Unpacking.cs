using InstallerLib.Installer.InstallCommand.Interfaces;
using InstallerLib.Progress;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstallerLib.Installer.InstallCommand.Unpacking
{
    static class Unpacking
    {
        public static IEnumerable<string> GetFiles(byte[] archive)
        {
            using (var stream = new MemoryStream(archive))
            using (var arch = ZipFile.Read(stream))
                return arch.Entries.Select(x => x.FileName);
        }

        public static void Unpack(string path, byte[] data, Func<bool> stop, EventHandler<ProgressEventArgs> eventHandler, object sender, double progressStart, double progressEnd)
        {
            string prevFile = null;
            using (var stream = new MemoryStream(data))
            using (var archive = ZipFile.Read(stream))
            {
                archive.ExtractProgress += (o, e) =>
                {
                    if (stop != null && stop())
                        e.Cancel = true;

                    if (e.CurrentEntry != null && prevFile != e.CurrentEntry.FileName)
                    {
                        prevFile = e.CurrentEntry.FileName;

                        eventHandler.Invoke(sender, new ProgressEventArgs($"Распаковка: {e.CurrentEntry.FileName}", progressStart + (progressEnd - progressStart) * e.EntriesExtracted / e.EntriesTotal));
                    }
                };
                archive.ExtractAll(path, ExtractExistingFileAction.OverwriteSilently);
            }
        }

    }
}
