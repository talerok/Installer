using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.Compiler
{
    public class CompilerException : Exception
    {
        public CompilerException(string msg) : base(msg) { }
    }
}
