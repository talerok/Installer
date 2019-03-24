using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.InstallCodeGenerators
{
    public static class ZipArchiveExtension
    {

        public static void CreateEntryFromAny(this ZipArchive archive, String sourceName, String entryName = "")
        {
            var fileName = Path.GetFileName(sourceName);
            if (File.GetAttributes(sourceName).HasFlag(FileAttributes.Directory))
                archive.CreateEntryFromDirectory(sourceName, Path.Combine(entryName, fileName));
            else
            {
                archive.CreateEntryFromFile(sourceName, Path.Combine(entryName, fileName), CompressionLevel.Fastest);
            }
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, String sourceDirName, String entryName = "")
        {
            string[] files = Directory.GetFiles(sourceDirName).Concat(Directory.GetDirectories(sourceDirName)).ToArray();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                archive.CreateEntryFromAny(file, entryName);
            }
        }

    }
}
