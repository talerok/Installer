using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Assembler.CodeGenerator.Console
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


    class JSONConsoleInstallCodeGenerator
    {
        private const string _appStartCommandName = "appStart";
        private const string _registryCommandName = "registry";
        private const string _zipUnpackingCommnadName = "unpackZip";
        private const string _copyCommandName = "copy";

        private JSONConfig _config;

        public bool UseDefaultPath { get { return _config.UseDefaultPath; } }
        public string DefaultPath { get { return _config.DefaultPath; } }

        [DataContract]
        private class JSONCommandConfig
        {
            [DataMember]
            public string CommandType { get; set; }
            [DataMember]
            public string[] Params { get; set; }
        }

        [DataContract]
        private class JSONConfig
        {
            [DataMember]
            public bool UseDefaultPath { get; set; }
            [DataMember]
            public string DefaultPath { get; set; }
            [DataMember]
            public string RegistryVersionPath { get; set; }
            [DataMember]
            public string Version { get; set; }
            [DataMember]
            public string ForVersion { get; set; }
            [DataMember]
            public List<JSONCommandConfig> Commands { get; set; }
        }

        private static JSONConfig _readConfig(string path)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(JSONConfig));
            using (FileStream fstream = new FileStream(path, FileMode.Open))
            {
                var res = (JSONConfig)jsonFormatter.ReadObject(fstream);
                return res;
            }
        }

        private static string _generateUnpackCommand(string bundleName, string path)
        {
            return ObjectGenerator.Generate(
                            null,
                            "ZipBundleUnpacker",
                            StringGenerator.Generate(bundleName),
                            $@"installPath + {StringGenerator.Generate($@"\{path}")}",
                            $@"(byte[])_getResxByName({StringGenerator.Generate(bundleName)})");
        }

        private static string _generateCommandCode(JSONCommandConfig commandConfig, JSONConfig config, IDictionary<string, byte[]> resources)
        {
            if (commandConfig == null || commandConfig.Params == null || config == null)
                throw new ArgumentNullException("incorrect config");

            switch (commandConfig.CommandType)
            {
                case _appStartCommandName:
                    if (commandConfig.Params.Length >= 2)
                        return ObjectGenerator.Generate(null, "AutoStart", StringGenerator.Generate(commandConfig.Params[0]), StringGenerator.Generate(commandConfig.Params[1]));
                    break;
                case _registryCommandName:
                    if (commandConfig.Params.Length >= 4)
                        return ObjectGenerator.Generate(null, "SimpleRegisterCommand", StringGenerator.Generate(commandConfig.Params[0]), StringGenerator.Generate(commandConfig.Params[1]), StringGenerator.Generate(commandConfig.Params[2]), $"RegValueKind.{commandConfig.Params[3]}");
                    break;
                case _zipUnpackingCommnadName:
                    if (commandConfig.Params.Length >= 3)
                    {
                        var data = File.ReadAllBytes(commandConfig.Params[1]);
                        resources.Add(commandConfig.Params[0], data);
                        return _generateUnpackCommand(commandConfig.Params[0], commandConfig.Params[2]);
                    }
                    break;
                case _copyCommandName:
                    if (commandConfig.Params.Length >= 3)
                    {

                        using (var memoryStream = new MemoryStream())
                        {
                            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                                archive.CreateEntryFromAny(commandConfig.Params[1]);

                            resources.Add(commandConfig.Params[0], memoryStream.ToArray());
                            return _generateUnpackCommand(commandConfig.Params[0], commandConfig.Params[2]);
                        }
                    }
                    break;
            }
            throw new ArgumentException("incorrect command");
        }

        private static string _generateAdminCheckCode(JSONConfig config)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("adminCheck", "AdminCheck"));
            res.AppendLine($@"if(!adminCheck.Check())");
            res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("Инсталятор должен быть запущен от имени администратора")));
            return res.ToString();
        }

        private static string _generateVersionCheckCode(JSONConfig config)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("versionCheck", "GetVersion", StringGenerator.Generate(config.RegistryVersionPath)));
            res.AppendLine($@"var currentVersion = versionCheck.GetInfo();");
            if (string.IsNullOrEmpty(config.ForVersion))
            {
                res.AppendLine($@"if(currentVersion != null && currentVersion.CompareTo(""{config.Version}"") != -1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Уже установленна более поздняя версия программы (текущая версия: "" + currentVersion + "", версия установщика: {config.Version})"""));
            }
            else
            {
                res.AppendLine($@"if(currentVersion != ""{config.ForVersion}"")");
               res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Не подходящая версия программы (текущая версия: "" + currentVersion + "", необходимая версия: {config.ForVersion})"""));
                                
            }
            
            return res.ToString();
        }

        private static string _generateSelectDirCode(JSONConfig config)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("pathCheck", "GetPath", StringGenerator.Generate(config.RegistryVersionPath)));
            res.AppendLine($@"var installPath = pathCheck.GetInfo();");

            if (!string.IsNullOrEmpty(config.ForVersion))
            {
                res.AppendLine(@"if (installPath == null)");
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("не найден каталог установки")));
            }
            else
            {
                res.AppendLine(@"if (installPath == null) {");
                if (!config.UseDefaultPath)
                {
                    res.AppendLine(@"Console.WriteLine(""Введите директорию установки:"");");
                    res.AppendLine("installPath = Console.ReadLine();");
                    res.AppendLine("}");
                }
                else
                {
                    res.AppendLine($@"installPath = {config.DefaultPath};");
                    res.AppendLine("} else {");
                    res.AppendLine($@"commands.Add({ObjectGenerator.Generate(null, "ClearDirectory", "installPath")});");
                    res.AppendLine("}");
                }
            }
            return res.ToString();
        }

        public (string Code, Dictionary<string, byte[]> Resources) GetCode()
        {
            try
            {
                var code = new StringBuilder();
                var resources = new Dictionary<string, byte[]>();

                var commandsCode = new List<string>();
                commandsCode.AddRange(_config.Commands.Select(x => _generateCommandCode(x, _config, resources)));
                commandsCode.Add(ObjectGenerator.Generate(null, "SetVersion", StringGenerator.Generate(_config.Version), StringGenerator.Generate(_config.RegistryVersionPath)));
                commandsCode.Add(ObjectGenerator.Generate(null, "SetPath", "installPath", StringGenerator.Generate(_config.RegistryVersionPath)));
 
                var forBody = new StringBuilder();
                forBody.AppendLine("var command = commands[step];");
                forBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Выполняю: ")} + command.Description"));
                forBody.AppendLine("command.Do();");

                var foreachBody = "command.Finish();";

                code.AppendLine("List<IInstallCommand> commands = null;");
                code.AppendLine("int step = 0;");
                code.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Запуск установки")});"));
                code.AppendLine(TryGenerator.Generate(
                        $"{_generateAdminCheckCode(_config)}\n" +
                        $"{_generateVersionCheckCode(_config)}\n" +
                        $"{_generateSelectDirCode(_config)}\n" +
                        $"commands = {ListCodeGenerator.Generate(null, "IInstallCommand", commandsCode)}\n" +
                        $"{ForGenerator.Generate("", "step < commands.Count", "step++", forBody.ToString())}\n" +
                        $"{ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Финализация установки")}")}" +
                        $"{ForeachGenerator.Generate("command", "commands", foreachBody)}" +
                        $"{ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Установка прошла успешно")}")}"
                    )
                );

                var InstallCatchBody = new StringBuilder();
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Выполняю откат установки")}"));
                InstallCatchBody.AppendLine(ForGenerator.Generate("", "step >= 0", "step--", "commands[step].Undo();"));
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Откат выполнен")}"));
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Установка завершилась с ошибкой")}"));
                code.AppendLine(CatchGenerator.Generate("InstallException", "ex", InstallCatchBody.ToString()));

                var catchBody = new StringBuilder();
                catchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
                catchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Установка завершилась с ошибкой")}"));

                code.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

                code.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Нажмите любую клавишу для продолжения")}"));
                code.AppendLine("Console.ReadKey();");
                return (code.ToString(), resources);
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException(ex.Message);
            }
        }

        public JSONConsoleInstallCodeGenerator(string configPath)
        {
            _config = _readConfig(configPath);
        }
    }
}
