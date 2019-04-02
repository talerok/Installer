using Assembler.CodeGenerator.Form;
using Assembler.InstallConfig;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.Uninstaller
{
    public class UninstallerCodeGenerator
    {
        private Config _config;

        private static string _generateProccesTextPring(string text, bool quotes = false) => quotes ? $@"UninstallRichTextBox.AppendText({StringGenerator.Generate(text)} + ""\n"");" : $@"UninstallRichTextBox.AppendText({text} + ""\n"");";

        public UninstallerCodeGenerator(Config config)
        {
            _config = config;
        }

        private const string _designer = @"/// <summary>
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
                                                this.UninstallRichTextBox = new System.Windows.Forms.RichTextBox();
                                                this.UninstallProgressBar = new System.Windows.Forms.ProgressBar();
                                                this.SuspendLayout();
                                                // 
                                                // UninstallRichTextBox
                                                // 
                                                this.UninstallRichTextBox.Location = new System.Drawing.Point(12, 12);
                                                this.UninstallRichTextBox.Name = ""UninstallRichTextBox"";
                                                this.UninstallRichTextBox.ReadOnly = true;
                                                this.UninstallRichTextBox.Size = new System.Drawing.Size(678, 393);
                                                this.UninstallRichTextBox.TabIndex = 0;
                                                this.UninstallRichTextBox.Text = """";
                                                // 
                                                // UninstallProgressBar
                                                // 
                                                this.UninstallProgressBar.Location = new System.Drawing.Point(12, 411);
                                                this.UninstallProgressBar.Name = ""UninstallProgressBar"";
                                                this.UninstallProgressBar.Size = new System.Drawing.Size(678, 23);
                                                this.UninstallProgressBar.TabIndex = 1;
                                                // 
                                                // Form1
                                                // 
                                                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                                                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                                                this.BackColor = System.Drawing.SystemColors.Window;
                                                this.ClientSize = new System.Drawing.Size(702, 446);
                                                this.Controls.Add(this.UninstallProgressBar);
                                                this.Controls.Add(this.UninstallRichTextBox);
                                                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
                                                this.MaximizeBox = false;
                                                this.MinimizeBox = false;
                                                this.Name = ""Form1"";
                                                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                                                this.Text = ""Form1"";
                                                this.ResumeLayout(false);

                                            }

                                            #endregion

                                            private const int WS_SYSMENU = 0x80000;
                                            private RichTextBox UninstallRichTextBox;
                                            private ProgressBar UninstallProgressBar;

                                            protected override CreateParams CreateParams
                                            {
                                                get
                                                {
                                                    CreateParams cp = base.CreateParams;
                                                    cp.Style &= ~WS_SYSMENU;
                                                    return cp;
                                                }
                                            }";

       
        private string _generateInsileDeleteFolderMethod()
        {
            var res = new StringBuilder();
            res.AppendLine("var curFolder = System.Reflection.Assembly.GetExecutingAssembly().Location.ToLower();");
            res.AppendLine("var deleteFolder = uninstaller.GetUnstallPath().ToLower();");

            res.AppendLine("return curFolder.Contains(deleteFolder);");
            return MethodGenerator.Generate(new string[] { "private" }, "bool", "_insideDeleteFolder", new string[] { "Uninstaller uninstaller" }, res.ToString());
        }

        private string _generateRestartMethod()
        {
            var res = new StringBuilder();
            res.AppendLine($@"var fileName = {StringGenerator.Generate($@"{_config.AppName}-uninstaller.exe")};");
            res.AppendLine($@"var path = Path.GetTempPath();");
            res.AppendLine("File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, path + fileName, true);");

            res.AppendLine("var info = new System.Diagnostics.ProcessStartInfo() {");
            res.AppendLine("FileName = fileName,");
            res.AppendLine("WorkingDirectory = path");
            res.AppendLine("};");

            res.AppendLine("System.Diagnostics.Process.Start(info);");
            res.AppendLine("Environment.Exit(0);");

            res.AppendLine(_generateProccesTextPring("2", true));

            return MethodGenerator.Generate(new string[] { "private" }, "void", "_restart", new string[] { }, res.ToString());
        }

        private string _generateCheckMethod()
        {
            var res = new StringBuilder();

            res.AppendLine(ObjectGenerator.Generate("adminCheck", "AdminCheck"));

            var tryBody = new StringBuilder();

            tryBody.AppendLine($@"if(!adminCheck.Check())");
            tryBody.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("Деинсталлятор должен быть запущен от имени администратора")));

            tryBody.AppendLine("if(!uninstaller.Check())");
            tryBody.AppendLine(ThrowGenerator.Generate("Exception", StringGenerator.Generate("Программа не найдена")));

            tryBody.AppendLine("if(_insideDeleteFolder(uninstaller))");
            tryBody.AppendLine("_restart();");

            tryBody.AppendLine($@"if (MessageBox.Show({StringGenerator.Generate($"Вы действительно хотите удалить {_config.AppName}?")},
                {StringGenerator.Generate("Удаление программы")}, MessageBoxButtons.YesNo) == DialogResult.No)");
            tryBody.AppendLine("Environment.Exit(0);");

            res.AppendLine(TryGenerator.Generate(tryBody.ToString()));

            var catchBody = new StringBuilder();
            catchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка удаления программы"), "ex.Message"));
            catchBody.AppendLine("Environment.Exit(0);");
            res.AppendLine(CatchGenerator.Generate("Exception", "ex", catchBody.ToString()));

            return MethodGenerator.Generate(new string[] { "private" }, "void", "_check", new string[] { "Uninstaller uninstaller" }, res.ToString());
        }

        private string _generateProccesMethod()
        {
            var res = new StringBuilder();
            var tryBody = new StringBuilder();

            var eventHandlerBody = new StringBuilder();
            eventHandlerBody.AppendLine(_generateProccesTextPring("e.Message"));
            eventHandlerBody.AppendLine("UninstallProgressBar.Value = (int)e.Progress;");

            tryBody.AppendLine($@"uninstaller.UninstallEventHandler += (o, e) => {{ {eventHandlerBody.ToString()} }};");

            tryBody.AppendLine(_generateProccesTextPring($"Удаление программы {_config.AppName}", true));
            tryBody.AppendLine("uninstaller.Do();");
            tryBody.AppendLine(_generateProccesTextPring($"Финализация удаление программы", true));
            tryBody.AppendLine("uninstaller.Finish();");
            tryBody.AppendLine(InfoMessageBoxGenerator.Generate(StringGenerator.Generate("Удаление программы"), StringGenerator.Generate("Программа была успешно удалена")));
            res.AppendLine(TryGenerator.Generate(tryBody.ToString()));

            var unstallCatchBody = new StringBuilder();

            unstallCatchBody.AppendLine(_generateProccesTextPring("ex.Message"));
            unstallCatchBody.AppendLine(_generateProccesTextPring($"Востановление программы", true));
            unstallCatchBody.AppendLine("uninstaller.Undo();");
            unstallCatchBody.AppendLine(_generateProccesTextPring($"Программа востановлена", true));
            unstallCatchBody.AppendLine(ErrorMessageBoxGenerator.Generate(StringGenerator.Generate("Ошибка удаления программы"),  "ex.Message"));
            res.AppendLine(CatchGenerator.Generate("Exception", "ex", unstallCatchBody.ToString()));

            var finalyBody = new StringBuilder();
            finalyBody.AppendLine("Environment.Exit(0);");

            res.AppendLine(FinallyGenerator.Generate(finalyBody.ToString()));

            return MethodGenerator.Generate(new string[] { "private" }, "void", "_process", new string[] { "Uninstaller uninstaller" }, res.ToString());
        }

        private string _generateConstructor()
        {
            var res = new StringBuilder();
            res.AppendLine(ObjectGenerator.Generate("uninstaller", "Uninstaller", StringGenerator.Generate(_config.AppName)));

            res.AppendLine("InitializeComponent();");
            res.AppendLine("this.FormClosing += (o, e) => { e.Cancel = _preventClosing; };");
            res.AppendLine("_check(uninstaller);");  
            res.AppendLine($"this.Text = {StringGenerator.Generate($" Мастер удаления {_config.AppName}")};");
            res.AppendLine("this.Shown += (o, e) => { _process(uninstaller); };");
            return MethodGenerator.Generate(new string[] { "public" }, "", "Form1", new string[] { }, res.ToString());
        }

        public string Generate()
        {
            var res = new StringBuilder();
            res.AppendLine("private bool _preventClosing = true;");
            res.AppendLine(_generateInsileDeleteFolderMethod());
            res.AppendLine(_generateRestartMethod());
            res.AppendLine(_generateCheckMethod());
            res.AppendLine(_designer);
            res.AppendLine(_generateProccesMethod());
            res.AppendLine(_generateConstructor());

            return ClassGenerator.Generate(new string[] { "public" }, "Form1", res.ToString(), "Form");
        }
    }
}
