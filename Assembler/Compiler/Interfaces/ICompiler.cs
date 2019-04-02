using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.Compiler.Interfaces
{
    public interface ICompiler
    {
        void Compile(string path);
        void AddLocalLib(string libName);
    }
}
