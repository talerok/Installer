using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator.SimpleForm
{
    static class ErrorMessageBoxGenerator
    {
        public static string Generate(string title, string text)
        {
            return $@"MessageBox.Show({text},{title}, MessageBoxButtons.OK, MessageBoxIcon.Error); ";
        }
    }
}
