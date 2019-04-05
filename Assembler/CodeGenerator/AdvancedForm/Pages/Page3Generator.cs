using Assembler.CodeGenerator.InstallCodeGenerators;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Text;
using Localization;
using CodeGeneration.Components;

namespace Assembler.CodeGenerator.AdvancedForm.Pages
{
    static class Page3Generator
    {

        private static string _generateInstallProccesTextPring(string text, bool quotes = false) => quotes ? $@"InstallProccessTextBox.AppendText({StringGenerator.Generate(text)} + ""\n"");" : $@"InstallProccessTextBox.AppendText({text} + ""\n"");";

        private static string _generateShowNextPageMethod()
        {
            var res = new StringBuilder();
            res.AppendLine("LastPage.Path = Path;");
            res.AppendLine("LastPage.SetInformation(information);");
            res.AppendLine("if (blockButton)");
            res.AppendLine("LastPage.BlockStartCheckBox();");
            res.AppendLine("this.Visible = false;");
            res.AppendLine("LastPage.Visible = true;");
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_showNextPage", new string[] { "string information", "bool blockButton" }, res.ToString());
        }

        private static (string Code, Dictionary<string, byte[]> Resources) _generateInstallProcessMethod(Config config, BuildType buildType)
        {
            var resources = new Dictionary<string, byte[]>();

            var res = new StringBuilder();
            res.AppendLine(InstallProcessGenerator.Generate(config, buildType, resources, "Path", "DesktopShortCuts", "StartMenuShortCuts", "StartUp", "InstallProcessEventHandler", "_prevent"));


            return (MethodGenerator.Generate(new string[] { "private" }, "void", "_installProcess", new string[] {}, res.ToString()),
                    resources);
        }

        private static string _generateInstallEventMethod(Config config)
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
            body.AppendLine($@" _showNextPage({StringGenerator.Generate(config.AfterInstallMessage, false)}, false);");
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.FailInstall:");
            body.AppendLine($@"_showNextPage(""{Resources.InstallationError}: "" + (string)args.Info, true);");
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.CanceledInstall:");
            body.AppendLine($@"_showNextPage(""{Resources.InstallationCanceled}"", true);");
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.SetProgressMaxValue:");
            body.AppendLine("InstallProgressBar.Maximum = (int)args.Info;");
            body.AppendLine("break;");

            body.AppendLine("case InstallEventType.SetProgress:");
            body.AppendLine("InstallProgressBar.Value = (int)args.Info;");
            body.AppendLine("break;");
            body.AppendLine("}");

            var res = $@"this.BeginInvoke((MethodInvoker)(delegate {{ { body.ToString() } }}));";

            return InstallProcessGenerator.GenerateEventMethod(new string[] { "private" }, "_installEvent", "args", res);
        }

        private static string _generateStartInstallMethod()
        {
            return MethodGenerator.Generate(new string[] { "public" }, "void", "StartInstall", new string[] { }, "new Thread(new ThreadStart(_installProcess)).Start();");
        }

        private static string _generateConstructor()
        {
            var res = new StringBuilder();
            res.AppendLine("InitializeComponent();");
            res.AppendLine("this.Visible = false;");
            res.AppendLine("InstallProcessEventHandler += _installEvent;");
            res.AppendLine("PreventButton.Click += (o, e) => {");
            res.AppendLine($@"if (MessageBox.Show(""{Resources.CancelInstallQuestion}"", ""{Resources.InstallationMessageTittle}"", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)");
            res.AppendLine("_prevent = true;");
            res.AppendLine("};");
            return MethodGenerator.Generate(new string[] { "public" }, "", "Page3", new string[] { }, res.ToString());
        }

        public static (string Code, Dictionary<string, byte[]> Resources) Generate(Config config, BuildType buildType)
        {
            var res = new StringBuilder();

            res.AppendLine($@"private System.ComponentModel.IContainer components = null;

                            protected override void Dispose(bool disposing)
                            {{
                                if (disposing && (components != null))
                                {{
                                    components.Dispose();
                                }}
                                base.Dispose(disposing);
                            }}

                            private void InitializeComponent()
                            {{
                                this.InstallProccessTextBox = new System.Windows.Forms.RichTextBox();
                                this.InstallProgressBar = new System.Windows.Forms.ProgressBar();
                                this.PreventButton = new System.Windows.Forms.Button();
                                this.groupBox1 = new System.Windows.Forms.GroupBox();
                                this.groupBox1.SuspendLayout();
                                this.SuspendLayout();
                                // 
                                // InstallProccessTextBox
                                // 
                                this.InstallProccessTextBox.Location = new System.Drawing.Point(3, 3);
                                this.InstallProccessTextBox.Name = ""InstallProccessTextBox"";
                                this.InstallProccessTextBox.ReadOnly = true;
                                this.InstallProccessTextBox.Size = new System.Drawing.Size(504, 272);
                                this.InstallProccessTextBox.TabIndex = 5;
                                this.InstallProccessTextBox.Text = """";
                                // 
                                // InstallProgressBar
                                // 
                                this.InstallProgressBar.Location = new System.Drawing.Point(3, 281);
                                this.InstallProgressBar.Name = ""InstallProgressBar"";
                                this.InstallProgressBar.Size = new System.Drawing.Size(504, 23);
                                this.InstallProgressBar.TabIndex = 4;
                                // 
                                // PreventButton
                                // 
                                this.PreventButton.Location = new System.Drawing.Point(437, 11);
                                this.PreventButton.Name = ""PreventButton"";
                                this.PreventButton.Size = new System.Drawing.Size(75, 23);
                                this.PreventButton.TabIndex = 7;
                                this.PreventButton.Text = ""{Resources.CancelInstallButtonText}"";
                                this.PreventButton.UseVisualStyleBackColor = true;
                                // 
                                // groupBox1
                                // 
                                this.groupBox1.Controls.Add(this.PreventButton);
                                this.groupBox1.Location = new System.Drawing.Point(-5, 310);
                                this.groupBox1.Name = ""groupBox1"";
                                this.groupBox1.Size = new System.Drawing.Size(525, 40);
                                this.groupBox1.TabIndex = 13;
                                this.groupBox1.TabStop = false;
                                // 
                                // Page3
                                // 
                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                                this.BackColor = System.Drawing.SystemColors.Window;
                                this.Controls.Add(this.groupBox1);
                                this.Controls.Add(this.InstallProccessTextBox);
                                this.Controls.Add(this.InstallProgressBar);
                                this.Name = ""Page3"";
                                this.Size = new System.Drawing.Size(510, 350);
                                this.groupBox1.ResumeLayout(false);
                                this.ResumeLayout(false);

                            }}

                            private System.Windows.Forms.RichTextBox InstallProccessTextBox;
                            private System.Windows.Forms.ProgressBar InstallProgressBar;
                            private System.Windows.Forms.Button PreventButton;
                            private System.Windows.Forms.GroupBox groupBox1;");


            res.AppendLine(@"private bool _prevent = false;");
            res.AppendLine(@"public Page4 LastPage { get; set; }");
            res.AppendLine(@"public string Path { get; set; }");
            res.AppendLine(@"public bool DesktopShortCuts { get; set; }");
            res.AppendLine(@"public bool StartMenuShortCuts { get; set; }");
            res.AppendLine(@"public bool StartUp { get; set; }");

            res.AppendLine(_generateShowNextPageMethod());
            res.AppendLine(InstallProcessGenerator.GenerateAuxiliaryCode("InstallProcessEventHandler"));
            res.AppendLine(_generateInstallEventMethod(config));

            var instProcess = _generateInstallProcessMethod(config, buildType);
            res.AppendLine(instProcess.Code);

            res.AppendLine(_generateStartInstallMethod());
            res.AppendLine(_generateConstructor());

            return (ClassGenerator.Generate(new string[] { "public" }, "Page3", res.ToString(), "UserControl"),
                    instProcess.Resources);
        }
    }
}
