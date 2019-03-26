using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.InstallCodeGenerators
{
    static class InstallProcessGenerator
    {
        private static string _generateCommandCode(CommandConfig commandConfig, ref IDictionary<string, byte[]> resources)
        {
            if (commandConfig == null || commandConfig.Params == null)
                throw new ArgumentNullException("incorrect config");

            var res = InstallCommandGenerator.Generate(commandConfig.CommandName, commandConfig.Params, ref resources);

            if (res == null)
                throw new ArgumentException("incorrect command");

            return res;
        }

        private static string _generateEventInvoke(string EventType, string message = null, bool quotes = false)
        {
            if (message == null)
                return $"InstallProcessEventHandler.Invoke(this, new InstallProcessEventArgs(InstallEventType.{EventType}));";
            else
                if (!quotes)
                    return $"InstallProcessEventHandler.Invoke(this, new InstallProcessEventArgs(InstallEventType.{EventType}, {message}));";
                else
                    return $"InstallProcessEventHandler.Invoke(this, new InstallProcessEventArgs(InstallEventType.{EventType}, {StringGenerator.Generate(message)}));";
        }

        public static string GenerateAuxiliaryCode(string eventHandlerName)
        {
            return @"private enum InstallEventType
                    {
                        Message,
                        Error,
                        SetProgresMaxValue,
                        IncreaseProgress,
                        DecreaseProgress,
                        SuccesInstall,
                        FailInstall
                    }

                    private class InstallProcessEventArgs : EventArgs
                    {
                        public InstallEventType EventType { get; }
                        public object Info { get; }

                        public InstallProcessEventArgs(InstallEventType eventType)
                        {
                            EventType = eventType;
                        }

                        public InstallProcessEventArgs(InstallEventType eventType, object info)
                        {
                            EventType = eventType;
                            Info = info;
                        }
                    }

                    private EventHandler<InstallProcessEventArgs> " + eventHandlerName + ";";
        }

        public static string GenerateEventMethod(IEnumerable<string> modifiers, string methodName, string argsName, string body)
        {
            return MethodGenerator.Generate(modifiers, "void", methodName, new string[] { "object sender", $"InstallProcessEventArgs {argsName}" }, body);
        }

        public static string Generate(Config config, IDictionary<string,byte[]> resources, string installPathCode, string clearDirCode)
        {
            var code = new StringBuilder();
            code.AppendLine($@"var installPath = {installPathCode};");

            var commandsCode = new List<string>();
            commandsCode.AddRange(config.Commands.Select(x => _generateCommandCode(x, ref resources)));
            commandsCode.Add(ObjectGenerator.Generate(null, "SetVersion", StringGenerator.Generate(config.Version), StringGenerator.Generate(config.RegistryVersionPath)));
            commandsCode.Add(ObjectGenerator.Generate(null, "SetPath", "installPath", StringGenerator.Generate(config.RegistryVersionPath)));

            var forBody = new StringBuilder();
            forBody.AppendLine("var command = commands[step];");
            forBody.AppendLine(_generateEventInvoke("Message", "command.Description"));

            forBody.AppendLine(_generateEventInvoke("IncreaseProgress"));
            forBody.AppendLine("command.Do();");

            var foreachBody = "command.Finish();";

            code.AppendLine("List<IInstallCommand> commands = null;");
            code.AppendLine("int step = 0;");
            code.AppendLine(_generateEventInvoke("Message", "Запуск установки", true));

            var tryBody = new StringBuilder();
            tryBody.AppendLine($"commands = {ListCodeGenerator.Generate(null, "IInstallCommand", commandsCode)};");

            tryBody.AppendLine($"if({clearDirCode})");
            tryBody.AppendLine($@"commands.Insert(0, {ObjectGenerator.Generate(null, "ClearDirectory", "installPath")});");

            tryBody.AppendLine(_generateEventInvoke("SetProgresMaxValue", "commands.Count"));

            tryBody.AppendLine(ForGenerator.Generate("", "step < commands.Count", "step++", forBody.ToString()));
            tryBody.AppendLine(_generateEventInvoke("Message", "Финализация установки", true));
            tryBody.AppendLine(ForeachGenerator.Generate("command", "commands", foreachBody));
            tryBody.AppendLine(_generateEventInvoke("SuccesInstall"));

            code.AppendLine(TryGenerator.Generate(tryBody.ToString()));

            var InstallCatchBody = new StringBuilder();


            InstallCatchBody.AppendLine(_generateEventInvoke("Error", $"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
            InstallCatchBody.AppendLine(_generateEventInvoke("Error", "Выполняю откат установки", true));
            InstallCatchBody.AppendLine(ForGenerator.Generate("", "step >= 0", "step--", $"commands[step].Undo();\n{_generateEventInvoke("DecreaseProgress")};"));
            InstallCatchBody.AppendLine(_generateEventInvoke("Error", "Откат выполнен", true));
            InstallCatchBody.AppendLine(_generateEventInvoke("FailInstall", "ex.Message"));

            code.AppendLine(CatchGenerator.Generate("InstallException", "ex", InstallCatchBody.ToString()));

            var catchBody = new StringBuilder();
            catchBody.AppendLine(_generateEventInvoke("Error", $"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
            catchBody.AppendLine(_generateEventInvoke("FailInstall", "ex.Message"));
            code.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return code.ToString();
        }
    }
}
