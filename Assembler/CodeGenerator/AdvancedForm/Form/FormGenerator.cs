using Assembler.CodeGenerator.Form;
using Assembler.CodeGenerator.InstallCodeGenerators;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm.Form
{
    static class FormGenerator
    {

        private static string _generateLabel(Config config)
        {
            return StringGenerator.Generate($"Мастер установки {config.AppName} {config.Version}");
        }

        private static string _generatePrepareFormMethod(Config config, BuildType buildType)
        {
            var res = new StringBuilder();
            res.AppendLine($"this.Text = {_generateLabel(config)};");

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
                res.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("не найден каталог установки")));
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
            catchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка инициализации установщика"), "ex.Message"));
            catchBody.AppendLine("Environment.Exit(-1);");

            var res = new StringBuilder();
            res.AppendLine(TryGenerator.Generate(tryBody.ToString()));
            res.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return MethodGenerator.Generate(new string[] { "public" }, "", "Form1", new string[] { }, res.ToString());
        }

        public static string Generate(Config config, BuildType buildType)
        {
            var res = new StringBuilder();

            res.AppendLine(@"private System.ComponentModel.IContainer components = null;

                            protected override void Dispose(bool disposing)
                            {
                                if (disposing && (components != null))
                                {
                                    components.Dispose();
                                }
                                base.Dispose(disposing);
                            }

                            private void InitializeComponent()
                            {
                                this.SuspendLayout();
                                // 
                                // Form1
                                // 
                                 this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                                this.ClientSize = new System.Drawing.Size(511, 348);
                                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                this.MaximizeBox = false;
                                this.MinimizeBox = false;
                                this.Name = ""Form1"";
                                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                                this.Text = ""Form1"";
                                this.ResumeLayout(false);

                            }

                            private const int WS_SYSMENU = 0x80000;
                            protected override CreateParams CreateParams
                            {
                                get
                                {
                                    CreateParams cp = base.CreateParams;
                                    cp.Style &= ~WS_SYSMENU;
                                    return cp;
                                }
                            }");

            res.AppendLine(InstallProcessGenerator.GenerateVersionCheckMethod("_checkVersion", config, buildType));
            res.AppendLine(InstallProcessGenerator.GenerateAdminCheckMethod(config, buildType, "_checkAdmin"));

            res.AppendLine($@"private string _programName = {_generateLabel(config)};");
            res.AppendLine($@"private string _programInformation = {StringGenerator.Generate(config.Description, false)};");
            res.AppendLine(_generatePrepareFormMethod(config, buildType));
            res.AppendLine(_generateConstructor());

                            

            return ClassGenerator.Generate(new string[] { "public" }, "Form1", res.ToString(), "Form");
        }
    }
}
