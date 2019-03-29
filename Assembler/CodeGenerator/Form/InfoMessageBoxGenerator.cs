using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.Form
{
    static class InfoMessageBoxGenerator
    {
        public static string Generate(string title, string text)
        {
            return $@"MessageBox.Show({text},{title}, MessageBoxButtons.OK, MessageBoxIcon.Information); ";
        }
    }
}
