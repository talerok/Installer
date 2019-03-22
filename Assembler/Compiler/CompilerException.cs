using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.Compiler
{
    class CompilerException : Exception
    {
        public CompilerException(string msg) : base(msg) { }
    }
}
