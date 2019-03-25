using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Assembler.Substitute
{
    static class FoldersSubstitute
    {
        private static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static IEnumerable<FileInfo> Substitute(string path1, string path2)
        {
            var files1 = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories).Select(x => x.ToLower().Replace(path1.ToLower(), ""));
            var files2 = Directory.GetFiles(path2, "*.*", SearchOption.AllDirectories).Select(x => x.ToLower().Replace(path2.ToLower(), ""));

            var files = new List<string>();
            files.AddRange(files1);
            files.AddRange(files2);

            var res = new List<FileInfo>();

            foreach (var file in files.Distinct())
            {
                var file1Exists = files1.Any(x => x == file);
                var file2Exists = files2.Any(x => x == file);

                if (file1Exists && file2Exists)
                {
                    if (CalculateMD5(path1 + file) != CalculateMD5(path2 + file))
                        res.Add(new FileInfo { Path = file, Status = FileStatus.Modified });
                }
                else if(file1Exists && !file2Exists)
                    res.Add(new FileInfo { Path = file, Status = FileStatus.Deleted });
                else
                    res.Add(new FileInfo { Path = file, Status = FileStatus.Added });
            }
            return res;
        }
    }
}
