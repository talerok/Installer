using Assembler.InstallConfig;
using CodeGeneration.Components;
using Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm.Pages
{
    static class Page4Generator
    {
        private static bool _checkStartAfterInstall(Config config) => config.StartAfterInstall.Any();

        private static string _generateSetInformationMethod()
        {
            return MethodGenerator.Generate(new string[] { "public" }, "void", "SetInformation", new string[] { "string text" }, "Information.Text = text;");
        }

        private static string _generateBlockStartCheckBoxMethod()
        {
            return MethodGenerator.Generate(new string[] { "public" }, "void", "BlockStartCheckBox", new string[] {}, "StartProgramCheckBox.Enabled = false;");
        }

        private static string _generateStartProgrammMethod(Config config)
        {
            var res = new StringBuilder();

            if (_checkStartAfterInstall(config))
            {
                res.AppendLine("if(!StartProgramCheckBox.Checked)");
                res.AppendLine("return;");

                res.AppendLine($@"var installPath = Path;");
                res.AppendLine(ListCodeGenerator.Generate("paths", "string", config.StartAfterInstall.Select(x => StringGenerator.Generate(x))));
                res.AppendLine(ForeachGenerator.Generate("path", "paths", $@"System.Diagnostics.Process.Start(installPath + {StringGenerator.Generate(@"\")} + path);"));
            }
            return MethodGenerator.Generate(new string[] { "private" }, "void", "_startProgram", new string[] { }, res.ToString());
        }

        private static string _generateConstructor(Config config)
        {
            var res = new StringBuilder();
            res.AppendLine("InitializeComponent();");

            if (!_checkStartAfterInstall(config))
                res.AppendLine("StartProgramCheckBox.Visible = false;");

            res.AppendLine("Introduction.Text = introduction;");
            res.AppendLine("CloseButton.Click += (o, e) => {");
            res.AppendLine("if (StartProgramCheckBox.Enabled)");
            res.AppendLine("_startProgram();");
            res.AppendLine(" Environment.Exit(0);");
            res.AppendLine("};");
            return MethodGenerator.Generate(new string[] { "public" }, "", "Page4", new string[] { "string introduction" }, res.ToString());
        }

        public static string Generate(Config config)
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
                                this.CloseButton = new System.Windows.Forms.Button();
                                this.Introduction = new System.Windows.Forms.Label();
                                this.StartProgramCheckBox = new System.Windows.Forms.CheckBox();
                                this.groupBox1 = new System.Windows.Forms.GroupBox();
                                this.Information = new System.Windows.Forms.Label();
                                this.groupBox1.SuspendLayout();
                                this.SuspendLayout();
                                // 
                                // CloseButton
                                // 
                                this.CloseButton.Location = new System.Drawing.Point(437, 11);
                                this.CloseButton.Name = ""CloseButton"";
                                this.CloseButton.Size = new System.Drawing.Size(75, 23);
                                this.CloseButton.TabIndex = 8;
                                this.CloseButton.Text = ""{Resources.CloseButtonText}"";
                                this.CloseButton.UseVisualStyleBackColor = true;
                                // 
                                // Introduction
                                // 
                                this.Introduction.AutoSize = true;
                                this.Introduction.Font = new System.Drawing.Font(""Microsoft Sans Serif"", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                                this.Introduction.Location = new System.Drawing.Point(14, 12);
                                this.Introduction.Name = ""Introduction"";
                                this.Introduction.Size = new System.Drawing.Size(175, 24);
                                this.Introduction.TabIndex = 9;
                                // 
                                // StartProgramCheckBox
                                // 
                                this.StartProgramCheckBox.AutoSize = true;
                                this.StartProgramCheckBox.Location = new System.Drawing.Point(23, 15);
                                this.StartProgramCheckBox.Name = ""StartProgramCheckBox"";
                                this.StartProgramCheckBox.Size = new System.Drawing.Size(274, 17);
                                this.StartProgramCheckBox.TabIndex = 10;
                                this.StartProgramCheckBox.Text = ""{Resources.StartProgramAfterInstallText}"";
                                this.StartProgramCheckBox.UseVisualStyleBackColor = true;
                                // 
                                // groupBox1
                                // 
                                this.groupBox1.Controls.Add(this.CloseButton);
                                this.groupBox1.Controls.Add(this.StartProgramCheckBox);
                                this.groupBox1.Location = new System.Drawing.Point(-5, 310);
                                this.groupBox1.Name = ""groupBox1"";
                                this.groupBox1.Size = new System.Drawing.Size(525, 40);
                                this.groupBox1.TabIndex = 12;
                                this.groupBox1.TabStop = false;
                                // 
                                // Information
                                // 
                                this.Information.Location = new System.Drawing.Point(63, 51);
                                this.Information.Name = ""Information"";
                                this.Information.Size = new System.Drawing.Size(0, 13);
                                this.Information.TabIndex = 13;
                                this.Information.Size = new System.Drawing.Size(434, 245);
                                // 
                                // Page4
                                // 
                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                                this.BackColor = System.Drawing.SystemColors.Window;
                                this.Controls.Add(this.Information);
                                this.Controls.Add(this.groupBox1);
                                this.Controls.Add(this.Introduction);
                                this.Name = ""Page4"";
                                this.Size = new System.Drawing.Size(510, 350);
                                this.groupBox1.ResumeLayout(false);
                                this.groupBox1.PerformLayout();
                                this.ResumeLayout(false);
                                this.PerformLayout();

                            }}

                            private System.Windows.Forms.Button CloseButton;
                            private System.Windows.Forms.Label Introduction;
                            private System.Windows.Forms.CheckBox StartProgramCheckBox;
                            private System.Windows.Forms.GroupBox groupBox1;
                            private System.Windows.Forms.Label Information;");

            res.AppendLine("public string Path { get; set; }");
            res.AppendLine(_generateSetInformationMethod());
            res.AppendLine(_generateBlockStartCheckBoxMethod());
            res.AppendLine(_generateStartProgrammMethod(config));
            res.AppendLine(_generateConstructor(config));

            return ClassGenerator.Generate(new string[] { "public" }, "Page4", res.ToString(), "UserControl");
        }
    }
}
