using CodeGeneration.Components;
using Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm.Pages
{
    static class Page1Generator
    {
        public static string Generate()
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
                                this.NextButton = new System.Windows.Forms.Button();
                                this.CloseButton = new System.Windows.Forms.Button();
                                this.Introduction = new System.Windows.Forms.Label();
                                this.groupBox1 = new System.Windows.Forms.GroupBox();
                                this.Information = new System.Windows.Forms.Label();
                                this.groupBox1.SuspendLayout();
                                this.SuspendLayout();
                                // 
                                // NextButton
                                // 
                                this.NextButton.Location = new System.Drawing.Point(356, 11);
                                this.NextButton.Name = ""NextButton"";
                                this.NextButton.Size = new System.Drawing.Size(75, 23);
                                this.NextButton.TabIndex = 0;
                                this.NextButton.Text = ""{Resources.NextButtonText}"";
                                this.NextButton.UseVisualStyleBackColor = true;
                                // 
                                // CloseButton
                                // 
                                this.CloseButton.Location = new System.Drawing.Point(437, 11);
                                this.CloseButton.Name = ""CloseButton"";
                                this.CloseButton.Size = new System.Drawing.Size(75, 23);
                                this.CloseButton.TabIndex = 1;
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
                                this.Introduction.TabIndex = 2;
                                // 
                                // groupBox1
                                // 
                                this.groupBox1.Controls.Add(this.NextButton);
                                this.groupBox1.Controls.Add(this.CloseButton);
                                this.groupBox1.Location = new System.Drawing.Point(-5, 310);
                                this.groupBox1.Name = ""groupBox1"";
                                this.groupBox1.Size = new System.Drawing.Size(525, 40);
                                this.groupBox1.TabIndex = 4;
                                this.groupBox1.TabStop = false;
                                // 
                                // Information
                                // 
                                this.Information.Location = new System.Drawing.Point(63, 51);
                                this.Information.Name = ""Information"";
                                this.Information.Size = new System.Drawing.Size(0, 13);
                                this.Information.TabIndex = 5;
                                this.Information.Size = new System.Drawing.Size(434, 245);
                                // 
                                // Page1
                                // 
                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                                this.BackColor = System.Drawing.SystemColors.Window;
                                this.Controls.Add(this.Information);
                                this.Controls.Add(this.groupBox1);
                                this.Controls.Add(this.Introduction);
                                this.Name = ""Page1"";
                                this.Size = new System.Drawing.Size(510, 350);
                                this.groupBox1.ResumeLayout(false);
                                this.ResumeLayout(false);
                                this.PerformLayout();

                            }}

                            private System.Windows.Forms.Button NextButton;
                            private System.Windows.Forms.Button CloseButton;
                            private System.Windows.Forms.Label Introduction;
                            private System.Windows.Forms.GroupBox groupBox1;
                            private System.Windows.Forms.Label Information;");

            res.AppendLine(@"public UserControl NextPage { get; set; }
                            public Page1(string introduction, string information)
                            {
                                InitializeComponent();

                                Introduction.Text = introduction;
                                Information.Text = information;

                                NextButton.Click += (o, e) =>
                                {
                                    this.Visible = false;
                                    NextPage.Visible = true;
                                };

                                CloseButton.Click += (o, e) =>
                                {
                                    Environment.Exit(0);
                                };
                            }");

            return ClassGenerator.Generate(new string[] { "public" }, "Page1", res.ToString(), "UserControl");
        }
    }
}
