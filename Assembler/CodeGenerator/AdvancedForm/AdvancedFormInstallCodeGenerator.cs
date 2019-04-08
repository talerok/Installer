using Assembler.CodeGenerator.Form;
using Assembler.CodeGenerator.InstallCodeGenerators;
using Assembler.InstallConfig;
using CodeGeneration.Components;
using Common;
using Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm
{
    public class AdvancedFormGlobal
    {
        public Config Config { get; }
        public string FormCode { get; }
        public string InstallProccesCode { get; }

        public AdvancedFormGlobal(Config config, string formCode, string installProccesCode)
        {
            Config = config;
            FormCode = formCode;
            InstallProccesCode = installProccesCode;
        }
    }

    public class AdvancedFormInstallCodeGenerator
    {
        private Config _config;
        private BuildType _buildType;

        public AdvancedFormInstallCodeGenerator(Config config, BuildType buildType)
        {
            _config = config;
            _buildType = buildType;
        }

        private static string _generateLabel(Config config)
        {
            return Resources.InstallerName.GetFormated($@"{config.AppName} {config.Version}");
        }

        private static string _generatePrepareFormMethod(Config config, BuildType buildType)
        {
            var res = new StringBuilder();
            res.AppendLine($"this.Text = {StringGenerator.Generate(_generateLabel(config))};");

            //----------------
            res.AppendLine(ObjectGenerator.Generate("page1", "Page1", "_programName", "_programInformation"));
            res.AppendLine(ObjectGenerator.Generate("page2", "Page2", "_programName"));
            res.AppendLine(ObjectGenerator.Generate("page3", "Page3"));
            res.AppendLine(ObjectGenerator.Generate("page4", "Page4", "_programName"));

            res.AppendLine("page1.NextPage = page2;");
            res.AppendLine("page2.PrevPage = page1;");
            res.AppendLine("page2.InstallPage = page3;");
            res.AppendLine("page3.LastPage = page4;");

            res.AppendLine("this.Controls.Add(page1);");
            res.AppendLine("this.Controls.Add(page2);");
            res.AppendLine("this.Controls.Add(page3);");
            res.AppendLine("this.Controls.Add(page4);");

            //----------------

            res.AppendLine(ObjectGenerator.Generate("pathCheck", "GetPath", StringGenerator.Generate(config.AppName)));
            res.AppendLine($@"var installPath = pathCheck.GetInfo();");

            if (buildType == BuildType.Minor)
            {
                res.AppendLine(@"if (installPath == null)");
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate(Resources.InstallCatalogNotFound)));
                res.AppendLine("page2.Path = installPath;");
                res.AppendLine("page2.BlockSelectPath();");
            }
            else
            {
                if (config.MajorConfig.UseDefaultPath)
                    res.AppendLine("page2.BlockSelectPath();");

                res.AppendLine(@"if (installPath == null) {");
                res.AppendLine($"page2.Path = {StringGenerator.Generate(config.MajorConfig.DefaultPath)};");
                res.AppendLine("} else {");
                res.AppendLine("page2.Path = installPath;");
                res.AppendLine("page2.BlockSelectPath();");
                res.AppendLine("}");
            }
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_preapareForm", new string[] { }, res.ToString());
        }

        private static string _generateConstructor()
        {
            var tryBody = new StringBuilder();
            tryBody.AppendLine("_checkAdmin();");
            tryBody.AppendLine("InitializeComponent();");
            tryBody.AppendLine("_preapareForm();");
            tryBody.AppendLine("_checkVersion();");

            tryBody.AppendLine("this.FormClosing += (o, e) => { e.Cancel = true; };");

            var catchBody = new StringBuilder();
            catchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate(Resources.InstallerInitError), "ex.Message"));
            catchBody.AppendLine("Environment.Exit(-1);");

            var res = new StringBuilder();
            res.AppendLine(TryGenerator.Generate(tryBody.ToString()));
            res.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return MethodGenerator.Generate(new string[] { "public" }, "", "Form1", new string[] { }, res.ToString());
        }

        public (string Code, IDictionary<string, byte[]> Resources) GetCode()
        {
            var res = new StringBuilder();
            res.AppendLine(InstallProcessGenerator.GenerateVersionCheckMethod("_checkVersion", _config, _buildType));
            res.AppendLine(InstallProcessGenerator.GenerateAdminCheckMethod(_config, _buildType, "_checkAdmin"));

            res.AppendLine($@"private string _programName = {StringGenerator.Generate(_generateLabel(_config))};");
            res.AppendLine($@"private string _programInformation = {StringGenerator.Generate(_config.Description, false)};");
            res.AppendLine(_generatePrepareFormMethod(_config, _buildType));
            res.AppendLine(_generateConstructor());

            var resources = new Dictionary<string, byte[]>();
            var installProcces = InstallProcessGenerator.Generate(_config, _buildType, resources, "Path", "DesktopShortCuts", "StartMenuShortCuts", "StartUp", "InstallProcessEventHandler", "_prevent");

            var global = new AdvancedFormGlobal(_config, res.ToString(), installProcces);

            return (CodeGeneration.CodeGenerator.GenerateFromFile(@"Templates\AdvancedForm\Form\Form.cstemplate", global).Result, resources);
        }
    }
}
