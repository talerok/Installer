using Assembler.CodeGenerator.InstallCodeGenerators;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Assembler.InstallConfig;

namespace Assembler.CodeGenerator.Console
{
   
    class ConsoleInstallCodeGenerator
    {

        private Config _config;

        public bool UseDefaultPath { get { return _config.UseDefaultPath; } }
        public string DefaultPath { get { return _config.DefaultPath; } }


        private static string _generateCommandCode(CommandConfig commandConfig, ref IDictionary<string, byte[]> resources)
        {
            if (commandConfig == null || commandConfig.Params == null)
                throw new ArgumentNullException("incorrect config");

            var res = InstallCommandGenerator.Generate(commandConfig.CommandName, commandConfig.Params, ref resources);
            
            if(res == null)
                throw new ArgumentException("incorrect command");

            return res;
        }

        private static string _generateAdminCheckCode(Config config)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("adminCheck", "AdminCheck"));
            res.AppendLine($@"if(!adminCheck.Check())");
            res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("Инсталятор должен быть запущен от имени администратора")));
            return res.ToString();
        }

        private static string _generateVersionCheckCode(Config config)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("versionCheck", "GetVersion", StringGenerator.Generate(config.RegistryVersionPath)));
            res.AppendLine($@"var currentVersion = versionCheck.GetInfo();");
            if (string.IsNullOrEmpty(config.MinVersion) && string.IsNullOrEmpty(config.MaxVersion))
            {
                res.AppendLine($@"if(currentVersion != null && currentVersion.CompareTo(""{config.Version}"") != -1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Установленна более поздняя версия программы (текущая версия: "" + currentVersion + "", версия установщика: {config.Version})"""));
            }
            if(!string.IsNullOrEmpty(config.MinVersion))
            {
               res.AppendLine($@"if(currentVersion.CompareTo({StringGenerator.Generate(config.MinVersion)}) == -1)");
               res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Не подходящая версия программы (текущая версия: "" + currentVersion + "", минимальная возможная версия: {config.MinVersion})"""));
            }
            if (!string.IsNullOrEmpty(config.MaxVersion))
            {
                res.AppendLine($@"if(currentVersion.CompareTo({StringGenerator.Generate(config.MaxVersion)}) == 1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Не подходящая версия программы (текущая версия: "" + currentVersion + "", максимальная возможная версия: {config.MaxVersion})"""));
            }
            return res.ToString();
        }

        private static string _generateSelectDirCode(Config config)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("pathCheck", "GetPath", StringGenerator.Generate(config.RegistryVersionPath)));
            res.AppendLine($@"var installPath = pathCheck.GetInfo();");

            if (!string.IsNullOrEmpty(config.MinVersion) && !string.IsNullOrEmpty(config.MaxVersion))
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

        public (string Code, IDictionary<string, byte[]> Resources) GetCode()
        {
            try
            {
                var code = new StringBuilder();
                IDictionary<string, byte[]> resources = new Dictionary<string, byte[]>();

                var commandsCode = new List<string>();
                commandsCode.AddRange(_config.Commands.Select(x => _generateCommandCode(x, ref resources)));
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

                var tryBody = new StringBuilder();
                tryBody.AppendLine(_generateAdminCheckCode(_config));
                tryBody.AppendLine(_generateAdminCheckCode(_config));
                tryBody.AppendLine(_generateSelectDirCode(_config));
                tryBody.AppendLine($"commands = {ListCodeGenerator.Generate(null, "IInstallCommand", commandsCode)};");
                tryBody.AppendLine(ForGenerator.Generate("", "step < commands.Count", "step++", forBody.ToString()));
                tryBody.AppendLine(ConsolePrintGenerator.Generate("Финализация установки", true));
                tryBody.AppendLine(ForeachGenerator.Generate("command", "commands", foreachBody));
                tryBody.Append(ConsolePrintGenerator.Generate("Установка прошла успешно", true));
                code.AppendLine(TryGenerator.Generate(tryBody.ToString()));
                
                var InstallCatchBody = new StringBuilder();
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate("Выполняю откат установки", true));
                InstallCatchBody.AppendLine(ForGenerator.Generate("", "step >= 0", "step--", "commands[step].Undo();"));
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate("Откат выполнен", true));
                InstallCatchBody.AppendLine(ConsolePrintGenerator.Generate("Установка завершилась с ошибкой", true));
                code.AppendLine(CatchGenerator.Generate("InstallException", "ex", InstallCatchBody.ToString()));

                var catchBody = new StringBuilder();
                catchBody.AppendLine(ConsolePrintGenerator.Generate($"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
                catchBody.AppendLine(ConsolePrintGenerator.Generate("Установка завершилась с ошибкой", true));

                code.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

                code.AppendLine(ConsolePrintGenerator.Generate("Нажмите любую клавишу для продолжения", true));
                code.AppendLine("Console.ReadKey();");
                return (code.ToString(), resources);
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException(ex.Message);
            }
        }

        public ConsoleInstallCodeGenerator(Config config)
        {
            _config = config;
        }
    }
}
