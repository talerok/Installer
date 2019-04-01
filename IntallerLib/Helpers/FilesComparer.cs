using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InstallerLib.Helpers
{
    static class FilesComparer
    {

        private static string _checkMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Encoding.Default.GetString(md5.ComputeHash(stream));
                }
            }
        }

        private static string _checkMD5(byte[] file)
        {
            using (var md5 = MD5.Create())
                return Encoding.Default.GetString(md5.ComputeHash(file));
        }

        public static bool Compare(byte[] file1, byte[] file2)
        {
            return _checkMD5(file1) == _checkMD5(file2);
        }

        public static bool Compare(string file1, string file2)
        {
            return _checkMD5(file1) == _checkMD5(file2);
        }
    }
}
