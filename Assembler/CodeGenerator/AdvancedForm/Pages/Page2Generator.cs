using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler.CodeGenerator.AdvancedForm.Pages
{
    static class Page2Generator
    {
        public static string Generate(Config config, BuildType buildType)
        {
            var res = new StringBuilder();

            res.AppendLine(@"/// <summary> 
                            /// Обязательная переменная конструктора.
                            /// </summary>
                            private System.ComponentModel.IContainer components = null;

                            /// <summary> 
                            /// Освободить все используемые ресурсы.
                            /// </summary>
                            /// <param name=""disposing"">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
                            protected override void Dispose(bool disposing)
                            {
                                if (disposing && (components != null))
                                {
                                    components.Dispose();
                                }
                                base.Dispose(disposing);
                            }

                            #region Код, автоматически созданный конструктором компонентов

                            /// <summary> 
                            /// Требуемый метод для поддержки конструктора — не изменяйте 
                            /// содержимое этого метода с помощью редактора кода.
                            /// </summary>
                            private void InitializeComponent()
                            {
                                this.CloseButton = new System.Windows.Forms.Button();
                                this.NextButton = new System.Windows.Forms.Button();
                                this.BackButton = new System.Windows.Forms.Button();
                                this.SelectPathButton = new System.Windows.Forms.Button();
                                this.pathTextBox = new System.Windows.Forms.TextBox();
                                this.Introduction = new System.Windows.Forms.Label();
                                this.groupBox1 = new System.Windows.Forms.GroupBox();
                                this.groupBox2 = new System.Windows.Forms.GroupBox();
                                this.DesktopShortCutCheckBox = new System.Windows.Forms.CheckBox();
                                this.StartMenuShortCutCheckBox = new System.Windows.Forms.CheckBox();
                                this.StartUpCheckBox = new System.Windows.Forms.CheckBox();
                                this.groupBox1.SuspendLayout();
                                this.groupBox2.SuspendLayout();
                                this.SuspendLayout();
                                // 
                                // CloseButton
                                // 
                                this.CloseButton.Location = new System.Drawing.Point(437, 11);
                                this.CloseButton.Name = ""CloseButton"";
                                this.CloseButton.Size = new System.Drawing.Size(75, 23);
                                this.CloseButton.TabIndex = 3;
                                this.CloseButton.Text = ""Закрыть"";
                                this.CloseButton.UseVisualStyleBackColor = true;
                                // 
                                // NextButton
                                // 
                                this.NextButton.Enabled = false;
                                this.NextButton.Location = new System.Drawing.Point(356, 11);
                                this.NextButton.Name = ""NextButton"";
                                this.NextButton.Size = new System.Drawing.Size(75, 23);
                                this.NextButton.TabIndex = 2;
                                this.NextButton.Text = ""Установить"";
                                this.NextButton.UseVisualStyleBackColor = true;
                                // 
                                // BackButton
                                // 
                                this.BackButton.Location = new System.Drawing.Point(275, 11);
                                this.BackButton.Name = ""BackButton"";
                                this.BackButton.Size = new System.Drawing.Size(75, 23);
                                this.BackButton.TabIndex = 4;
                                this.BackButton.Text = ""Назад"";
                                this.BackButton.UseVisualStyleBackColor = true;
                                // 
                                // SelectPathButton
                                // 
                                this.SelectPathButton.Location = new System.Drawing.Point(382, 16);
                                this.SelectPathButton.Name = ""SelectPathButton"";
                                this.SelectPathButton.Size = new System.Drawing.Size(73, 23);
                                this.SelectPathButton.TabIndex = 6;
                                this.SelectPathButton.Text = ""Обзор..."";
                                this.SelectPathButton.UseVisualStyleBackColor = true;
                                // 
                                // pathTextBox
                                // 
                                this.pathTextBox.Location = new System.Drawing.Point(14, 18);
                                this.pathTextBox.Name = ""pathTextBox"";
                                this.pathTextBox.Size = new System.Drawing.Size(362, 20);
                                this.pathTextBox.TabIndex = 5;
                                // 
                                // Introduction
                                // 
                                this.Introduction.AutoSize = true;
                                this.Introduction.Font = new System.Drawing.Font(""Microsoft Sans Serif"", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                                this.Introduction.Location = new System.Drawing.Point(14, 12);
                                this.Introduction.Name = ""Introduction"";
                                this.Introduction.Size = new System.Drawing.Size(175, 24);
                                this.Introduction.TabIndex = 9;
                                this.Introduction.Text = ""Мастер установки"";
                                // 
                                // groupBox1
                                // 
                                this.groupBox1.Controls.Add(this.CloseButton);
                                this.groupBox1.Controls.Add(this.NextButton);
                                this.groupBox1.Controls.Add(this.BackButton);
                                this.groupBox1.Location = new System.Drawing.Point(-5, 310);
                                this.groupBox1.Name = ""groupBox1"";
                                this.groupBox1.Size = new System.Drawing.Size(525, 40);
                                this.groupBox1.TabIndex = 10;
                                this.groupBox1.TabStop = false;
                                // 
                                // groupBox2
                                // 
                                this.groupBox2.Controls.Add(this.pathTextBox);
                                this.groupBox2.Controls.Add(this.SelectPathButton);
                                this.groupBox2.Location = new System.Drawing.Point(20, 148);
                                this.groupBox2.Name = ""groupBox2"";
                                this.groupBox2.Size = new System.Drawing.Size(461, 45);
                                this.groupBox2.TabIndex = 11;
                                this.groupBox2.TabStop = false;
                                this.groupBox2.Text = ""Папка установки"";
                                // 
                                // DesktopShortCutCheckBox
                                // 
                                this.DesktopShortCutCheckBox.AutoSize = true;
                                this.DesktopShortCutCheckBox.Location = new System.Drawing.Point(20, 208);
                                this.DesktopShortCutCheckBox.Name = ""DesktopShortCutCheckBox"";
                                this.DesktopShortCutCheckBox.Size = new System.Drawing.Size(196, 17);
                                this.DesktopShortCutCheckBox.TabIndex = 12;
                                this.DesktopShortCutCheckBox.Text = ""Создать ярлык на рабочем столе"";
                                this.DesktopShortCutCheckBox.UseVisualStyleBackColor = true;
                                // 
                                // StartMenuShortCutCheckBox
                                // 
                                this.StartMenuShortCutCheckBox.AutoSize = true;
                                this.StartMenuShortCutCheckBox.Location = new System.Drawing.Point(20, 231);
                                this.StartMenuShortCutCheckBox.Name = ""StartMenuShortCutCheckBox"";
                                this.StartMenuShortCutCheckBox.Size = new System.Drawing.Size(169, 17);
                                this.StartMenuShortCutCheckBox.TabIndex = 13;
                                this.StartMenuShortCutCheckBox.Text = ""Создать ярлык в меню пуск"";
                                this.StartMenuShortCutCheckBox.UseVisualStyleBackColor = true;
                                // 
                                // StartUpCheckBox
                                // 
                                this.StartUpCheckBox.AutoSize = true;
                                this.StartUpCheckBox.Location = new System.Drawing.Point(20, 254);
                                this.StartUpCheckBox.Name = ""StartUpCheckBox"";
                                this.StartUpCheckBox.Size = new System.Drawing.Size(205, 17);
                                this.StartUpCheckBox.TabIndex = 14;
                                this.StartUpCheckBox.Text = ""Добавить программу в автозапуск"";
                                this.StartUpCheckBox.UseVisualStyleBackColor = true;
                                // 
                                // Page2
                                // 
                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                                this.BackColor = System.Drawing.SystemColors.Window;
                                this.Controls.Add(this.StartUpCheckBox);
                                this.Controls.Add(this.StartMenuShortCutCheckBox);
                                this.Controls.Add(this.DesktopShortCutCheckBox);
                                this.Controls.Add(this.groupBox2);
                                this.Controls.Add(this.groupBox1);
                                this.Controls.Add(this.Introduction);
                                this.Name = ""Page2"";
                                this.Size = new System.Drawing.Size(510, 350);
                                this.groupBox1.ResumeLayout(false);
                                this.groupBox2.ResumeLayout(false);
                                this.groupBox2.PerformLayout();
                                this.ResumeLayout(false);
                                this.PerformLayout();

                            }

                            #endregion

                            private System.Windows.Forms.Button CloseButton;
                            private System.Windows.Forms.Button NextButton;
                            private System.Windows.Forms.Button BackButton;
                            private System.Windows.Forms.Button SelectPathButton;
                            private System.Windows.Forms.TextBox pathTextBox;
                            private System.Windows.Forms.Label Introduction;
                            private System.Windows.Forms.GroupBox groupBox1;
                            private System.Windows.Forms.GroupBox groupBox2;
                            private System.Windows.Forms.CheckBox DesktopShortCutCheckBox;
                            private System.Windows.Forms.CheckBox StartMenuShortCutCheckBox;
                            private System.Windows.Forms.CheckBox StartUpCheckBox;");

            res.AppendLine(@"public Page3 InstallPage { get; set; }
                            public UserControl PrevPage { get; set; }

                            public string Path
                            {
                                get
                                {
                                    return pathTextBox.Text;
                                }
                                set
                                {
                                    if (!String.IsNullOrEmpty(value))
                                        NextButton.Enabled = true;
                                    pathTextBox.Text = value;
                                }
                            }

                            public void BlockSelectPath()
                            {
                                SelectPathButton.Enabled = false;
                            }");

            var configureCheckBoxesMethodBody = new StringBuilder();

            int _checkboxedCount = 0;

            if (buildType == BuildType.Minor || !config.MajorConfig.ShortCutsConfig.DesktopShortCuts.Any())
                configureCheckBoxesMethodBody.AppendLine("DesktopShortCutCheckBox.Visible = false;");
            else
            {
                configureCheckBoxesMethodBody.AppendLine($"DesktopShortCutCheckBox.Location = new System.Drawing.Point(20, {(208 + 23 * _checkboxedCount).ToString()});");
                _checkboxedCount++;
            }

            if (buildType == BuildType.Minor || !config.MajorConfig.ShortCutsConfig.StartMenuShortCuts.Any())
                configureCheckBoxesMethodBody.AppendLine("StartMenuShortCutCheckBox.Visible = false;");
            else
            {
                configureCheckBoxesMethodBody.AppendLine($"StartMenuShortCutCheckBox.Location = new System.Drawing.Point(20, {(208 + 23 * _checkboxedCount).ToString()});");
                _checkboxedCount++;
            }

            if (buildType == BuildType.Minor || !config.MajorConfig.ShortCutsConfig.AutoStart.Any())
                configureCheckBoxesMethodBody.AppendLine("StartUpCheckBox.Visible = false;");
            else
            {
                configureCheckBoxesMethodBody.AppendLine($"StartUpCheckBox.Location = new System.Drawing.Point(20, {(208 + 23 * _checkboxedCount).ToString()});");
                _checkboxedCount++;
            }

            res.AppendLine(MethodGenerator.Generate(new string[] { "private" }, "void", "_configureCheckBoxes", new string[] { }, configureCheckBoxesMethodBody.ToString()));

            res.AppendLine(@"public Page2(string introduction)
                            {
                                InitializeComponent();

                                this.Visible = false;
                                
                                _configureCheckBoxes();

                                Introduction.Text = introduction;

                                NextButton.Click += (o, e) =>
                                {
                                    this.Visible = false;
                                    InstallPage.Visible = true;
                                    InstallPage.Path = Path;
                                    InstallPage.DesktopShortCuts = DesktopShortCutCheckBox.Checked;
                                    InstallPage.StartMenuShortCuts = StartMenuShortCutCheckBox.Checked;
                                    InstallPage.StartUp = StartUpCheckBox.Checked;
                                    InstallPage.StartInstall();
                                };

                                BackButton.Click += (o, e) =>
                                {
                                    this.Visible = false;
                                    PrevPage.Visible = true;
                                };

                                CloseButton.Click += (o, e) =>
                                {
                                    Environment.Exit(0);
                                };
                            
                                if (String.IsNullOrEmpty(Path))
                                    NextButton.Enabled = false;
                                
                                pathTextBox.ReadOnly = true;

                                SelectPathButton.Click += (o, e) =>
                                {
                                    using (var dialog = new FolderBrowserDialog())
                                        if (dialog.ShowDialog() == DialogResult.OK)
                                        {
                                            pathTextBox.Text = dialog.SelectedPath;
                                            NextButton.Enabled = true;
                                        }
                                };

                            }");

            return ClassGenerator.Generate(new string[] { "public" }, "Page2", res.ToString(), "UserControl");
        }
    }
}
