using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Assembler.CodeGenerator.InstallCodeGenerators
{
    static class InstallCommandGenerator
    {
        private const string _appStartCommandName = "appStart";
        private const string _registryCommandName = "registry";
        private const string _zipUnpackingCommnadName = "unpackZip";
        private const string _copyCommandName = "copy";

        private static string _generateUnpackCommand(string bundleName, string path)
        {
            return ObjectGenerator.Generate(
                            null,
                            "ZipBundleUnpacker",
                            StringGenerator.Generate(bundleName),
                            $@"installPath + {StringGenerator.Generate($@"\{path}")}",
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
                    var data = File.ReadAllBytes(commandParams[1]);
                    resources.Add(commandParams[0], data);
                    return _generateUnpackCommand(commandParams[0], commandParams[2]);
                case _copyCommandName:
                    if (commandParams.Length < 3)
                        return null; 
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            archive.CreateEntryFromAny(commandParams[1]);

                        resources.Add(commandParams[0], memoryStream.ToArray());
                        return _generateUnpackCommand(commandParams[0], commandParams[2]);
                    }
                default:
                    return null;
            }
        }
    }
}
