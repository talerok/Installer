using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    public class CodeGeneratorException : Exception
    {
        public CodeGeneratorException(string msg) : base(msg)
        {

        }
    }
}
