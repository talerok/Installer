using Assembler.InstallConfig;
using Common;
using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.InstallCodeGenerators
{
    static class InstallProcessGenerator
    {
       
        private static string _generateEventInvoke(string EventHandler, string EventType, string message = null, bool quotes = false)
        {
            if (message == null)
                return $"{EventHandler}.Invoke(this, new InstallProcessEventArgs(InstallEventType.{EventType}));";
            else
                if (!quotes)
                    return $"{EventHandler}.Invoke(this, new InstallProcessEventArgs(InstallEventType.{EventType}, {message}));";
                else
                    return $"{EventHandler}.Invoke(this, new InstallProcessEventArgs(InstallEventType.{EventType}, {StringGenerator.Generate(message)}));";
        }

        private static string _generateShortCutCommand(Config config, IEnumerable<ShortCutConfig> shortCuts, string commandName)
        {
            var listBody = shortCuts.Select(x => ObjectGenerator.Generate(null, "ShortCutInfo", StringGenerator.Generate(x.Name), $@"installPath + {StringGenerator.Generate($@"\{x.FilePath}")}"));
            var list = ListCodeGenerator.Generate(null, "ShortCutInfo", listBody);
            return ObjectGenerator.Generate(null, commandName, StringGenerator.Generate(config.AppName), list);
        }

        public static string GenerateAuxiliaryCode(string eventHandlerName)
        {
            return @"private enum InstallEventType
                    {
                        Message,
                        Error,
                        SetProgress,
                        SetProgressMaxValue,
                        SuccesInstall,
                        FailInstall,
                        CanceledInstall
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

        public static string Generate(Config config, BuildType buildType, IDictionary<string,byte[]> resources, string installPathCode, string desktopShortCutsCode, string startMenuShortCutsCode, string startUpCode, string eventHandler, string stopInstallCode = null)
        {
            var code = new StringBuilder();
            code.AppendLine($@"var installPath = {installPathCode};");

            var commandsCode = new List<string>();

            if (!string.IsNullOrEmpty(stopInstallCode))
                commandsCode.Add(InstallCommandGenerator.Generate(config, buildType, ref resources, $"() => {stopInstallCode}" )); // () => {stopInstallCode} - функция отмены разархивирования
            else
                commandsCode.Add(InstallCommandGenerator.Generate(config, buildType, ref resources, $"null"));
            commandsCode.Add(ObjectGenerator.Generate(null, "SetVersion", StringGenerator.Generate(config.Version), "installPath"));


            var undoBody = ForGenerator.Generate("", "step >= 0", "step--", $"commands[step].Undo();");

            var forBody = new StringBuilder();
            forBody.AppendLine("var command = commands[step];");
            forBody.AppendLine("command.InstallProgressEventHandler += (o,a) => {"); // Подписываемся на события комманд установки
            forBody.AppendLine(_generateEventInvoke(eventHandler, "Message", "a.Message", false));
            forBody.AppendLine("if(step == 0)"); 
            forBody.AppendLine(_generateEventInvoke(eventHandler, "SetProgress", "(int)(a.Progress)", false)); // учитываем в прогрессе только распаковку (так проще)
            forBody.AppendLine("};"); 
            forBody.AppendLine("command.Do();");

            if (!string.IsNullOrEmpty(stopInstallCode)) // Отмена установки
            {
                forBody.AppendLine($"if({stopInstallCode}) {{");
                forBody.AppendLine(_generateEventInvoke(eventHandler, "Message", Resources.CancelInstallMessageText, true));
                forBody.AppendLine(_generateEventInvoke(eventHandler, "Message", Resources.RevertInstallMessageText, true));
                forBody.AppendLine(undoBody);
                forBody.AppendLine(_generateEventInvoke(eventHandler, "CanceledInstall"));
                forBody.AppendLine("return;");
                forBody.AppendLine("}");
            }
            var foreachBody = "command.Finish();";

            code.AppendLine("List<IInstallCommand> commands = null;");
            code.AppendLine("int step = 0;");
            code.AppendLine(_generateEventInvoke(eventHandler, "Message", Resources.InstallationStartMessageText, true));

            var tryBody = new StringBuilder();

            tryBody.AppendLine($"commands = {ListCodeGenerator.Generate(null, "IInstallCommand", commandsCode)};");

            tryBody.AppendLine(ObjectGenerator.Generate("pCheck", "GetPath", StringGenerator.Generate(config.AppName)));

            tryBody.AppendLine("if(pCheck.GetInfo() == null) {"); // первая установка
            tryBody.AppendLine($@"commands.Add({ObjectGenerator.Generate(null, "SetPath", StringGenerator.Generate(config.AppName), "installPath")});");
            tryBody.AppendLine($@"commands.Add({ObjectGenerator.Generate(null, "RegisterProgram", 
                StringGenerator.Generate(config.AppName), 
                StringGenerator.Generate(config.Version),
                StringGenerator.Generate(config.CompanyName),
                $"installPath + {StringGenerator.Generate($@"\uninstaller.exe")}"
            )});");
            tryBody.AppendLine("}");

            if (buildType == BuildType.Major)
            {

                tryBody.AppendLine($"if({desktopShortCutsCode})");
                tryBody.AppendLine($@"commands.Add({_generateShortCutCommand(config, config.MajorConfig.ShortCutsConfig.DesktopShortCuts, "Desktop")});");

                tryBody.AppendLine($"if({startMenuShortCutsCode})");
                tryBody.AppendLine($@"commands.Add({_generateShortCutCommand(config, config.MajorConfig.ShortCutsConfig.StartMenuShortCuts, "StartMenu")});");

                tryBody.AppendLine($"if({startUpCode})");
                tryBody.AppendLine($@"commands.Add({_generateShortCutCommand(config, config.MajorConfig.ShortCutsConfig.AutoStart, "AutoStart")});");

            }

            tryBody.AppendLine(ForGenerator.Generate("", "step < commands.Count", "step++", forBody.ToString()));
            tryBody.AppendLine(_generateEventInvoke(eventHandler, "Message", Resources.InstallFinalizationMessageText, true));
            tryBody.AppendLine(ForeachGenerator.Generate("command", "commands", foreachBody));
            tryBody.AppendLine(_generateEventInvoke(eventHandler, "SuccesInstall"));

            code.AppendLine(TryGenerator.Generate(tryBody.ToString()));

            var InstallCatchBody = new StringBuilder();


            InstallCatchBody.AppendLine(_generateEventInvoke(eventHandler, "Error", $"{StringGenerator.Generate($"{Resources.InstallationErrorMessageText}: ")} + ex.Message"));
            InstallCatchBody.AppendLine(_generateEventInvoke(eventHandler, "Error", Resources.RevertInstallMessageText, true));
            InstallCatchBody.AppendLine(undoBody);
            InstallCatchBody.AppendLine(_generateEventInvoke(eventHandler, "FailInstall", "ex.Message"));

            code.AppendLine(CatchGenerator.Generate("InstallException", "ex", InstallCatchBody.ToString()));

            var catchBody = new StringBuilder();
            catchBody.AppendLine(_generateEventInvoke(eventHandler, "Error", $"{StringGenerator.Generate($"{Resources.InstallationErrorMessageText}: ")} + ex.Message"));
            catchBody.AppendLine(_generateEventInvoke(eventHandler, "FailInstall", "ex.Message"));
            code.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return code.ToString();
        }

        public static string GenerateAdminCheckMethod(Config config, BuildType buildType, string methodName)
        {
            var body = new StringBuilder();

            if(buildType == BuildType.Minor)
                return MethodGenerator.Generate(new string[] { "private" }, "void", methodName, new string[] { }, "");

            body.AppendLine(ObjectGenerator.Generate("adminCheck", "AdminCheck"));
            body.AppendLine($@"if(!adminCheck.Check())");
            body.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate(Resources.NeedAdminMessageText)));

            var res = new StringBuilder();

            if (config.MajorConfig.ShortCutsConfig.AutoStart.Any() ||
                config.MajorConfig.ShortCutsConfig.DesktopShortCuts.Any() ||
                config.MajorConfig.ShortCutsConfig.StartMenuShortCuts.Any()) // Нужны права админа чтобы создавать ярлыки
            {
                res.AppendLine(body.ToString());
            }
            else
            {
                res.AppendLine(ObjectGenerator.Generate("pCheck", "GetPath", StringGenerator.Generate(config.AppName))); // Нужны права админа при первой установки
                res.AppendLine("if(pCheck.GetInfo() == null) {"); // т.е если нет информации по пути установки -> программа не была установлена
                res.AppendLine(body.ToString());
                res.AppendLine("}");
            }
           
            return MethodGenerator.Generate(new string[] { "private" }, "void", methodName, new string[] { }, res.ToString());
        }

        public static string GenerateVersionCheckMethod(string methodName, Config config, BuildType buildType)
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("versionCheck", "GetVersion", StringGenerator.Generate(config.AppName)));
            res.AppendLine($@"var currentVersion = versionCheck.GetInfo();");
            if (buildType == BuildType.Major)
            {
                res.AppendLine($@"if(currentVersion != null && currentVersion.CompareTo(""{config.Version}"") != -1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate(
                    Resources.VersionErrorMessageText1.GetFormated(
                        StringGenerator.Generate(" + currentVersion + ", false), 
                        config.Version), false)));
            }
            else
            {
                res.AppendLine($@"if(currentVersion == null || currentVersion != {StringGenerator.Generate(config.MinorConfig.ForVersion)})");
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate(
                    Resources.VersionErrorMessageText2.GetFormated(
                        StringGenerator.Generate(@" + (currentVersion != null ? currentVersion : ""отсутствует"") + ", false),
                        config.Version), false)));
            }
           
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_checkVersion", new string[] { }, res.ToString());
        }

    }
}
