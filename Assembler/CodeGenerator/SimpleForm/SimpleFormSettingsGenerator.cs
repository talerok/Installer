using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.SimpleForm
{
    static class SimpleFormSettingsGenerator
    {
        public static string Generate()
        {
            return @"/// <summary>
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

                    #region Код, автоматически созданный конструктором форм Windows

                    /// <summary>
                    /// Требуемый метод для поддержки конструктора — не изменяйте 
                    /// содержимое этого метода с помощью редактора кода.
                    /// </summary>
                    private void InitializeComponent()
                    {
                        this.InstallProgressBar = new System.Windows.Forms.ProgressBar();
                        this.pathTextBox = new System.Windows.Forms.TextBox();
                        this.SelectPathButton = new System.Windows.Forms.Button();
                        this.InstallProccessTextBox = new System.Windows.Forms.RichTextBox();
                        this.InstallButton = new System.Windows.Forms.Button();
                        this.SuspendLayout();
                        // 
                        // InstallProgressBar
                        // 
                        this.InstallProgressBar.Location = new System.Drawing.Point(12, 321);
                        this.InstallProgressBar.Name = ""InstallProgressBar"";
                        this.InstallProgressBar.Size = new System.Drawing.Size(460, 23);
                        this.InstallProgressBar.TabIndex = 0;
                        // 
                        // pathTextBox
                        // 
                        this.pathTextBox.Location = new System.Drawing.Point(12, 12);
                        this.pathTextBox.Name = ""pathTextBox"";
                        this.pathTextBox.ReadOnly = true;
                        this.pathTextBox.Size = new System.Drawing.Size(356, 20);
                        this.pathTextBox.TabIndex = 1;
                        // 
                        // SelectPathButton
                        // 
                        this.SelectPathButton.Location = new System.Drawing.Point(374, 10);
                        this.SelectPathButton.Name = ""SelectPathButton"";
                        this.SelectPathButton.Size = new System.Drawing.Size(98, 23);
                        this.SelectPathButton.TabIndex = 2;
                        this.SelectPathButton.Text = ""Выбор папки"";
                        this.SelectPathButton.UseVisualStyleBackColor = true;
                        this.SelectPathButton.Click += new System.EventHandler(this.SelectPathButton_Click);
                        // 
                        // InstallProccessTextBox
                        // 
                        this.InstallProccessTextBox.Location = new System.Drawing.Point(12, 41);
                        this.InstallProccessTextBox.Name = ""InstallProccessTextBox"";
                        this.InstallProccessTextBox.ReadOnly = true;
                        this.InstallProccessTextBox.Size = new System.Drawing.Size(460, 274);
                        this.InstallProccessTextBox.TabIndex = 3;
                        this.InstallProccessTextBox.Text = """";
                        // 
                        // InstallButton
                        // 
                        this.InstallButton.Location = new System.Drawing.Point(374, 350);
                        this.InstallButton.Name = ""InstallButton"";
                        this.InstallButton.Size = new System.Drawing.Size(98, 23);
                        this.InstallButton.TabIndex = 4;
                        this.InstallButton.Text = ""Установить"";
                        this.InstallButton.UseVisualStyleBackColor = true;
                        this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
                        // 
                        // Form1
                        // 
                        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                        this.ClientSize = new System.Drawing.Size(484, 381);
                        this.Controls.Add(this.InstallButton);
                        this.Controls.Add(this.InstallProccessTextBox);
                        this.Controls.Add(this.SelectPathButton);
                        this.Controls.Add(this.pathTextBox);
                        this.Controls.Add(this.InstallProgressBar);
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                        this.Name = ""Form1"";
                        this.ResumeLayout(false);
                        this.PerformLayout();

                    }

                    #endregion

                    private System.Windows.Forms.ProgressBar InstallProgressBar;
                    private System.Windows.Forms.TextBox pathTextBox;
                    private System.Windows.Forms.Button SelectPathButton;
                    private System.Windows.Forms.RichTextBox InstallProccessTextBox;
                    private System.Windows.Forms.Button InstallButton;";
        }
    }
}
