﻿using Assembler.InstallConfig;
using CodeGeneration;
using CodeGeneration.Components;
using Common;
using Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Assembler.CodeGenerator.InstallCodeGenerators
{
    static class InstallCommandGenerator
    {
        private const string _appStartCommandName = "appStart";
        private const string _registryCommandName = "registry";
        private const string _zipUnpackingCommnadName = "unpackZip";
        private const string _copyCommandName = "copy";
        private const string _substituteCommandName = "substitute";

        private static string _generateUnpackCommand(string stopCode)
        {
            return ObjectGenerator.Generate(
                            null,
                            "Major",
                            $@"installPath",
                            $@"(byte[])_getResxByName({StringGenerator.Generate("archive")})",
                            stopCode);
        }

        private static string _generateDeletedFilesList(IEnumerable<Common.FileInfo> info)
        {
            return ListCodeGenerator.Generate(null, "string", info.Where(x => x.Status == FileStatus.Deleted).Select(x => StringGenerator.Generate(x.Path)));
        }

        private static string _generateReplaceCommand(string stopCode, IEnumerable<Common.FileInfo> info)
        {
            return ObjectGenerator.Generate(
                            null,
                            "Minor",
                            $@"installPath",
                            _generateDeletedFilesList(info),
                            $@"(byte[])_getResxByName({StringGenerator.Generate("archive")})",
                            stopCode);
        }

        public static string Generate(Config config, BuildType buildType, ref IDictionary<string, byte[]> resources, string stopCode)
        {
            if (buildType == BuildType.Major)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        archive.CreateEntryFromDirectory(config.BuildPath);

                    resources.Add("archive", memoryStream.ToArray());
                }

                return _generateUnpackCommand(stopCode);
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (Directory.Exists("buffer"))
                        Directory.Delete("buffer", true);
                    Directory.CreateDirectory("buffer");

                    var prevVersionFolder = VersionPath.Generate(config.AppName, config.MinorConfig.ForVersion);
                    if (!Directory.Exists(prevVersionFolder))
                        throw new CodeGeneratorException(Resources.VersionFolderNotFound.GetFormated(config.MinorConfig.ForVersion, prevVersionFolder));

                    var filesInfo = FoldersSubstitute.Substitute(prevVersionFolder, config.BuildPath);

                    foreach (var fileInfo in filesInfo.Where(x => x.Status == FileStatus.Added || x.Status == FileStatus.Modified))
                    {
                        var folderPath = @"buffer\" + Path.GetDirectoryName(fileInfo.Path);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);
                        File.Copy(config.BuildPath + fileInfo.Path, @"buffer\" + fileInfo.Path);
                    }

                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        archive.CreateEntryFromDirectory("buffer");
                    resources.Add("archive", memoryStream.ToArray());

                    if (Directory.Exists("buffer"))
                        Directory.Delete("buffer", true);

                    return _generateReplaceCommand(stopCode, filesInfo);
                }
            }
        }
    }
}
