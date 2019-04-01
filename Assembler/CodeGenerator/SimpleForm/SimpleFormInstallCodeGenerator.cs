using Assembler.CodeGenerator.Form;
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

        private string _generateInstallProccesTextPring(string text, bool quotes = false) => quotes ? $@"InstallProccessTextBox.AppendText({StringGenerator.Generate(text)} + ""\n"");" : $@"InstallProccessTextBox.AppendText({text} + ""\n"");";

        private bool _checkStartAfterInstall() => _config.StartAfterInstall.Any();


        private string _generatePrepareFormMethod()
        {
            var res = new StringBuilder();
            var formName = StringGenerator.Generate($"Установщик {_config.AppName} Version {_config.Version}");
            res.AppendLine($"this.Text = {formName};");

            res.AppendLine(ObjectGenerator.Generate("pathCheck", "GetPath", StringGenerator.Generate(_config.AppName)));
            res.AppendLine($@"var installPath = pathCheck.GetInfo();");

            if (!_checkStartAfterInstall())
                res.AppendLine("StartProgramCheckBox.Visible = false;");



            if (!string.IsNullOrEmpty(_config.ForVersion))
            {
                res.AppendLine(@"if (installPath == null)");
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("не найден каталог установки")));
                res.AppendLine("pathTextBox.Text = installPath;");
                res.AppendLine("SelectPathButton.Enabled = false;");
                res.AppendLine("InstallButton.Enabled = true;");
            }
            else
            {
                if (_config.UseDefaultPath)
                    res.AppendLine("SelectPathButton.Enabled = false;");

                res.AppendLine(@"if (installPath == null) {");
                res.AppendLine($"pathTextBox.Text = {StringGenerator.Generate(_config.DefaultPath)};");
                if(!String.IsNullOrEmpty(_config.DefaultPath))
                    res.AppendLine("InstallButton.Enabled = true;");

                res.AppendLine("} else {");
                res.AppendLine("pathTextBox.Text = installPath;");
                res.AppendLine("InstallButton.Enabled = true;");
                res.AppendLine("SelectPathButton.Enabled = false;");
                res.AppendLine("}");
            }
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_preapareForm", new string[] { }, res.ToString());
        }

        private string _generateFormConstructor()
        {
            var tryBody = new StringBuilder();
            tryBody.AppendLine("_checkAdmin();");
            tryBody.AppendLine("InitializeComponent();");
            tryBody.AppendLine("_preapareForm();");
            tryBody.AppendLine("_checkVersion();");
            tryBody.AppendLine("InstallProcessEventHandler += _installEvent;");

            tryBody.AppendLine(_generateInstallProccesTextPring(StringGenerator.Generate(_config.Description, false)));
          
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
            res.AppendLine("if (dialog.ShowDialog() == DialogResult.OK) {");
            res.AppendLine("pathTextBox.Text = dialog.SelectedPath;");
            res.AppendLine("InstallButton.Enabled = true;");
            res.AppendLine("}");
            return MethodGenerator.Generate(new string[] { "private" }, "void", "SelectPathButton_Click", new string[] { "object sender", "EventArgs e" }, res.ToString());
        }

        private string _generateInstallButtonClickMethod(IDictionary<string, byte[]> resources)
        {
            var code = new StringBuilder("_blockButtons();");
            code.AppendLine(InstallProcessGenerator.Generate(_config, resources, "pathTextBox.Text", "true", "true", "true", "InstallProcessEventHandler"));
            code.AppendLine("CloseButton.Enabled = true;");

            return MethodGenerator.Generate(new string[] { "private" }, "void", "InstallButton_Click", new string[] { "object sender", "EventArgs e" }, code.ToString());
        }

        private string _generateInstallEventMethod()
        {
            var body = new StringBuilder();

            body.AppendLine("switch(args.EventType) {");

            body.AppendLine("case InstallEventType.Message:");
            body.AppendLine(_generateInstallProccesTextPring("(string)args.Info"));
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.Error:");
            body.AppendLine(_generateInstallProccesTextPring("(string)args.Info"));
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.SuccesInstall:");
            body.AppendLine(_generateInstallProccesTextPring(StringGenerator.Generate(_config.AfterInstallMessage, false)));
            body.AppendLine(InfoMessageBoxGenerator.Generate(StringGenerator.Generate("Установка"), StringGenerator.Generate(_config.AfterInstallMessage)));
            body.AppendLine("_startProgram();");
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.FailInstall:");
            body.AppendLine(_generateInstallProccesTextPring("Установка завершилась с ошибкой", true));
            body.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка установки"), "(string)args.Info"));
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.SetProgressMaxValue:");
            body.AppendLine("InstallProgressBar.Maximum = (int)args.Info;");
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.SetProgress:");
            body.AppendLine("InstallProgressBar.Value = (int)args.Info;");
            body.AppendLine("break;");
            body.AppendLine("}");
            return InstallProcessGenerator.GenerateEventMethod(new string[] { "private" }, "_installEvent", "args", body.ToString());
        }

        private string _generateCloseButtonClickMethod()
        {
            return MethodGenerator.Generate(new string[] { "private" }, "void", "CloseButton_Click", new string[] { "object sender", "EventArgs e" }, "Environment.Exit(0);");
        }

        private string _generateStartProgrammMethod()
        {
            var res = new StringBuilder();

            if (_checkStartAfterInstall())
            {
                res.AppendLine("if(!StartProgramCheckBox.Checked)");
                res.AppendLine("return;");

                res.AppendLine($@"var installPath = pathTextBox.Text;");
                res.AppendLine(ListCodeGenerator.Generate("paths", "string", _config.StartAfterInstall.Select(x => StringGenerator.Generate(x))));
                res.AppendLine(ForeachGenerator.Generate("path", "paths", $@"System.Diagnostics.Process.Start(installPath + {StringGenerator.Generate(@"\")} + path);"));
            }
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_startProgram", new string[] { }, res.ToString());
        }
       
        private string _generateBlockButtonsMethod()
        {
            var res = new StringBuilder();
            res.AppendLine("SelectPathButton.Enabled = false;");
            res.AppendLine("InstallButton.Enabled = false;");
            res.AppendLine("CloseButton.Enabled = false;");
            if(_checkStartAfterInstall())
                res.AppendLine("StartProgramCheckBox.Enabled = false;");
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_blockButtons", new string[] { }, res.ToString());
        }

        public (string Code, IDictionary<string, byte[]> Resources) GetCode()
        {
            try
            {
                IDictionary<string, byte[]> resources = new Dictionary<string, byte[]>();

                var formClassBody = new StringBuilder();

                formClassBody.AppendLine(SimpleFormSettingsGenerator.Generate());
                formClassBody.AppendLine(InstallProcessGenerator.GenerateAuxiliaryCode("InstallProcessEventHandler"));
                formClassBody.AppendLine(_generateInstallEventMethod());
                formClassBody.AppendLine(_generatePrepareFormMethod());
                formClassBody.AppendLine(InstallProcessGenerator.GenerateVersionCheckMethod("_versionCheck", _config));
                formClassBody.AppendLine(InstallProcessGenerator.GenerateAdminCheckMethod(_config, "_checkAdmin"));
                formClassBody.AppendLine(_generateFormConstructor());
                formClassBody.AppendLine(_generateSelectPathButtonClickMethod());
                formClassBody.AppendLine(_generateCloseButtonClickMethod());
                formClassBody.AppendLine(_generateInstallButtonClickMethod(resources));
                formClassBody.AppendLine(_generateBlockButtonsMethod());
                formClassBody.AppendLine(_generateStartProgrammMethod());

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
