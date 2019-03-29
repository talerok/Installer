using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.Compiler.Interfaces
{
    interface ICompiler
    {
        void Compile(string path);
        void AddLocalLib(string libName);
    }
}
