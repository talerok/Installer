using Assembler.Substitute;
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

        private static string _generateUnpackCommand(string bundleName, string path)
        {
            return ObjectGenerator.Generate(
                            null,
                            "ZipBundleUnpacker",
                            StringGenerator.Generate(bundleName),
                            $@"installPath + {StringGenerator.Generate($@"\{path}")}",
                            $@"(byte[])_getResxByName({StringGenerator.Generate(bundleName)})");
        }

        private static string _generateDeletedFilesList(IEnumerable<Assembler.Substitute.FileInfo> info)
        {
            return ListCodeGenerator.Generate(null, "string", info.Where(x => x.Status == FileStatus.Deleted).Select(x => StringGenerator.Generate(x.Path)));
        }

        private static string _generateReplaceCommand(string bundleName, string path, IEnumerable<Assembler.Substitute.FileInfo> info)
        {
            return ObjectGenerator.Generate(
                            null,
                            "Replace",
                            StringGenerator.Generate(bundleName),
                            $@"installPath + {StringGenerator.Generate($@"\{path}")}",
                            _generateDeletedFilesList(info),
                            $@"(byte[])_getResxByName({StringGenerator.Generate(bundleName)})");
        }

        public static string Generate(string commandName, string[] commandParams, ref IDictionary<string, byte[]> resources)
        {
            switch (commandName)
            {
                case _appStartCommandName:
                    if (commandParams.Length < 2)
                        return null;
                    return ObjectGenerator.Generate(null, "AutoStart", StringGenerator.Generate(commandParams[0]), StringGenerator.Generate(commandParams[1]));
                case _registryCommandName:
                    if (commandParams.Length < 4)
                        return null;
                    return ObjectGenerator.Generate(null, "SimpleRegisterCommand", StringGenerator.Generate(commandParams[0]), StringGenerator.Generate(commandParams[1]), StringGenerator.Generate(commandParams[2]), $"RegValueKind.{commandParams[3]}");
                case _zipUnpackingCommnadName:
                    if (commandParams.Length < 3)
                        return null;
                    var data = File.ReadAllBytes(commandParams[2]);
                    resources.Add(commandParams[0], data);
                    return _generateUnpackCommand(commandParams[0], commandParams[1]);
                case _copyCommandName:
                    if (commandParams.Length < 3)
                        return null; 
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            archive.CreateEntryFromDirectory(commandParams[2]);

                        resources.Add(commandParams[0], memoryStream.ToArray());
                        return _generateUnpackCommand(commandParams[0], commandParams[1]);
                    }
                case _substituteCommandName:
                    if (commandParams.Length < 4)
                        return null;

                    var filesInfo = FoldersSubstitute.Substitute(commandParams[2], commandParams[3]);
                    try
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            if (Directory.Exists("buffer"))
                                Directory.Delete("buffer", true);
                            Directory.CreateDirectory("buffer");

                            foreach (var fileInfo in filesInfo.Where(x => x.Status == FileStatus.Added || x.Status == FileStatus.Modified))
                            {
                                var folderPath = @"buffer\" + Path.GetDirectoryName(fileInfo.Path);
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);
                                File.Copy(commandParams[3] + fileInfo.Path, @"buffer\" + fileInfo.Path);
                            }

                            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                                archive.CreateEntryFromDirectory("buffer");
                            resources.Add(commandParams[0], memoryStream.ToArray());

                            return _generateReplaceCommand(commandParams[0], commandParams[1], filesInfo);
                        }
                    }catch(Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (Directory.Exists("buffer"))
                            Directory.Delete("buffer", true);
                    }
                default:
                    return null;
            }
        }
    }
}
