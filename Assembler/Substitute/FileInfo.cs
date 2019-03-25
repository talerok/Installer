using System;
using System.Collections.Generic;
using System.Text;

namespace Assembler.Substitute
{
    enum FileStatus{
        Added,
        Deleted,
        Modified
    }

    class FileInfo
    {
        public string Path { get; set; }
        public FileStatus Status { get; set; }
    }
}
