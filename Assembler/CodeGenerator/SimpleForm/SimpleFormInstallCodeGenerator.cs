using Assembler.CodeGenerator.Console;
using Assembler.CodeGenerator.InstallCodeGenerators;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.SimpleForm
{
    class SimpleFormInstallCodeGenerator
    {
        private Config _config;

        public bool UseDefaultPath { get { return _config.UseDefaultPath; } }
        public string DefaultPath { get { return _config.DefaultPath; } }

        private string _generateInstallProccesTextPring(string text, bool quotes = false) => quotes ? $@"InstallProccessTextBox.AppendText({StringGenerator.Generate(text)} + ""\n"");" : $@"InstallProccessTextBox.AppendText({text} + ""\n"");";   

        private static string _generateCommandCode(CommandConfig commandConfig, ref IDictionary<string, byte[]> resources)
        {
            if (commandConfig == null || commandConfig.Params == null)
                throw new ArgumentNullException("incorrect config");

            var res = InstallCommandGenerator.Generate(commandConfig.CommandName, commandConfig.Params, ref resources);

            if (res == null)
                throw new ArgumentException("incorrect command");

            return res;
        }

        private string _generateAdminCheckMethod()
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("adminCheck", "AdminCheck"));
            res.AppendLine($@"if(!adminCheck.Check())");
            res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("Инсталятор должен быть запущен от имени администратора")));
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_checkAdmin", new string[] { }, res.ToString());
        }

        private string _generateVersionCheckMethod()
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("versionCheck", "GetVersion", StringGenerator.Generate(_config.RegistryVersionPath)));
            res.AppendLine($@"var currentVersion = versionCheck.GetInfo();");
            if (string.IsNullOrEmpty(_config.MinVersion) && string.IsNullOrEmpty(_config.MaxVersion))
            {
                res.AppendLine($@"if(currentVersion != null && currentVersion.CompareTo(""{_config.Version}"") != -1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Установленна более поздняя версия программы (текущая версия: "" + currentVersion + "", версия установщика: {_config.Version})"""));
            }
            if (!string.IsNullOrEmpty(_config.MinVersion))
            {
                res.AppendLine($@"if(currentVersion.CompareTo({StringGenerator.Generate(_config.MinVersion)}) == -1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Не подходящая версия программы (текущая версия: "" + currentVersion + "", минимальная возможная версия: {_config.MinVersion})"""));
            }
            if (!string.IsNullOrEmpty(_config.MaxVersion))
            {
                res.AppendLine($@"if(currentVersion.CompareTo({StringGenerator.Generate(_config.MaxVersion)}) == 1)");
                res.AppendLine(ThrowGenerator.Generate("Exception", $@"""Не подходящая версия программы (текущая версия: "" + currentVersion + "", максимальная возможная версия: {_config.MaxVersion})"""));
            }

            return MethodGenerator.Generate(new string[] { "private" }, "void", "_checkVersion", new string[] { }, res.ToString());
        }


        private string _generatePrepareFormMethod()
        {
            var res = new StringBuilder();
            var formName = StringGenerator.Generate($"Установщик {_config.AppName} Version {_config.Version}");
            res.AppendLine($"this.Text = {formName};");

            res.AppendLine(ObjectGenerator.Generate("pathCheck", "GetPath", StringGenerator.Generate(_config.RegistryVersionPath)));
            res.AppendLine($@"var installPath = pathCheck.GetInfo();");

            if (!string.IsNullOrEmpty(_config.MinVersion) && !string.IsNullOrEmpty(_config.MaxVersion))
            {
                res.AppendLine(@"if (installPath == null)");
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("не найден каталог установки")));
                res.AppendLine("pathTextBox.Text = installPath;");
                res.AppendLine("SelectPathButton.Enabled = false;");
            }
            else
            {
                if(_config.UseDefaultPath)
                    res.AppendLine("SelectPathButton.Enabled = false;");

                res.AppendLine(@"if (installPath == null) {");
                res.AppendLine($"pathTextBox.Text = {StringGenerator.Generate(_config.DefaultPath)};");
                res.AppendLine("} else {");
                res.AppendLine("pathTextBox.Text = installPath;");
                res.AppendLine("SelectPathButton.Enabled = false;");
                res.AppendLine("_clearDir = true;");
                res.AppendLine("}");
            }
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_preapareForm", new string[] { }, res.ToString());
        }

        private string _generateFormConstructor()
        {
            var tryBody = new StringBuilder();
            tryBody.AppendLine("_checkAdmin();");
            tryBody.AppendLine("_checkVersion();");
            tryBody.AppendLine("InitializeComponent();");
            tryBody.AppendLine("_preapareForm();");

            var catchBody = new StringBuilder();
            catchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка инициализации установщика"), "ex.Message"));
            catchBody.AppendLine("Environment.Exit(-1);");

            var res = new StringBuilder();
            res.AppendLine(TryGenerator.Generate(tryBody.ToString()));
            res.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return MethodGenerator.Generate(new string[] { "public" }, "", "Form1", new string[] { }, res.ToString());
        }

        private string _generateSelectPathButtonClickMethod()
        {
            var res = new StringBuilder();
            res.AppendLine("using (var dialog = new FolderBrowserDialog())");
            res.AppendLine("if (dialog.ShowDialog() == DialogResult.OK)");
            res.AppendLine("pathTextBox.Text = dialog.SelectedPath;");
            return MethodGenerator.Generate(new string[] { "private" }, "void", "SelectPathButton_Click", new string[] { "object sender", "EventArgs e" }, res.ToString());
        }

        private string _generateInstallButtonClickMethod(IDictionary<string, byte[]> resources)
        {
            var code = new StringBuilder();
            code.AppendLine("_blockButtons();");
            code.AppendLine($@"var installPath = pathTextBox.Text;");

            var commandsCode = new List<string>();
            commandsCode.AddRange(_config.Commands.Select(x => _generateCommandCode(x, ref resources)));
            commandsCode.Add(ObjectGenerator.Generate(null, "SetVersion", StringGenerator.Generate(_config.Version), StringGenerator.Generate(_config.RegistryVersionPath)));
            commandsCode.Add(ObjectGenerator.Generate(null, "SetPath", "installPath", StringGenerator.Generate(_config.RegistryVersionPath)));

            var forBody = new StringBuilder();
            forBody.AppendLine("var command = commands[step];");
            forBody.AppendLine(_generateInstallProccesTextPring("command.Description"));
            forBody.AppendLine("InstallProgressBar.Value++;");
            forBody.AppendLine("command.Do();");

            var foreachBody = "command.Finish();";

            code.AppendLine("List<IInstallCommand> commands = null;");
            code.AppendLine("int step = 0;");
            code.AppendLine(_generateInstallProccesTextPring("Запуск установки", true));

            var tryBody = new StringBuilder();
            tryBody.AppendLine($"commands = {ListCodeGenerator.Generate(null, "IInstallCommand", commandsCode)};");

            tryBody.AppendLine($"if(_clearDir)");
            tryBody.AppendLine($@"commands.Insert(0, {ObjectGenerator.Generate(null, "ClearDirectory", "installPath")});");

            tryBody.AppendLine($"InstallProgressBar.Maximum = commands.Count;");
            tryBody.AppendLine(ForGenerator.Generate("", "step < commands.Count", "step++", forBody.ToString()));
            tryBody.AppendLine(_generateInstallProccesTextPring("Финализация установки", true));
            tryBody.AppendLine(ForeachGenerator.Generate("command", "commands", foreachBody));
            tryBody.AppendLine(_generateInstallProccesTextPring("Установка прошла успешно", true));
            tryBody.AppendLine(InfoMessageBoxGenerator.Generate(StringGenerator.Generate("Установка"), StringGenerator.Generate("Установка прошла успешно")));
            code.AppendLine(TryGenerator.Generate(tryBody.ToString()));

            var InstallCatchBody = new StringBuilder();
            InstallCatchBody.AppendLine(_generateInstallProccesTextPring($"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
            InstallCatchBody.AppendLine(_generateInstallProccesTextPring("Выполняю откат установки", true));
            InstallCatchBody.AppendLine(ForGenerator.Generate("", "step >= 0", "step--", "commands[step].Undo();\nInstallProgressBar.Value--;"));
            InstallCatchBody.AppendLine(_generateInstallProccesTextPring("Откат выполнен", true));
            InstallCatchBody.AppendLine(_generateInstallProccesTextPring("Установка завершилась с ошибкой", true));
            InstallCatchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка установки"), "ex.Message"));
            code.AppendLine(CatchGenerator.Generate("InstallException", "ex", InstallCatchBody.ToString()));

            var catchBody = new StringBuilder();
            catchBody.AppendLine(_generateInstallProccesTextPring($"{StringGenerator.Generate("Ошибка установки: ")} + ex.Message"));
            catchBody.AppendLine(_generateInstallProccesTextPring("Установка завершилась с ошибкой", true));
            catchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка установки"), "ex.Message"));

            code.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return MethodGenerator.Generate(new string[] { "private" }, "void", "InstallButton_Click", new string[] { "object sender", "EventArgs e" }, code.ToString());
        }

        public (string Code, IDictionary<string, byte[]> Resources) GetCode()
        {
            try
            {
                IDictionary<string, byte[]> resources = new Dictionary<string, byte[]>();

                var blockButtonsMethodBody = new StringBuilder();
                blockButtonsMethodBody.AppendLine("SelectPathButton.Enabled = false;");
                blockButtonsMethodBody.AppendLine("InstallButton.Enabled = false;");


                var formClassBody = new StringBuilder();

                formClassBody.AppendLine("private bool _clearDir = false;");          
                formClassBody.AppendLine(SimpleFormSettingsGenerator.Generate());
                formClassBody.AppendLine(_generatePrepareFormMethod());
                formClassBody.AppendLine(_generateVersionCheckMethod());
                formClassBody.AppendLine(_generateAdminCheckMethod());
                formClassBody.AppendLine(_generateFormConstructor());
                formClassBody.AppendLine(_generateSelectPathButtonClickMethod());
                formClassBody.AppendLine(_generateInstallButtonClickMethod(resources));
                formClassBody.AppendLine(MethodGenerator.Generate(new string[] { "private" }, "void", "_blockButtons", new string[] { }, blockButtonsMethodBody.ToString()));

                return (ClassGenerator.Generate(new string[] { "public" }, "Form1", formClassBody.ToString(), "Form"), resources);
            }
            catch (Exception ex)
            {
                throw new CodeGeneratorException(ex.Message);
            }
        }

        public SimpleFormInstallCodeGenerator(Config config)
        {
            _config = config;
        }
    }
}
