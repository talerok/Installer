using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.CodeGenerator
{
    class CodeGeneratorException : Exception
    {
        public CodeGeneratorException(string msg) : base(msg)
        {

        }
    }
}
