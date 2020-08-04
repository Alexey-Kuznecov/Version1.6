using System;
using System.Collections.Generic;
using System.IO;

namespace UnityCommander.Core.IO
{
    public delegate bool SetCommand(FileInfo sourceFile);

    class DirectoryItems
    {
        public string Target { get; }
        public string Source { get; }
        public bool MD5Enable { get; set; }
        public bool Exists { get; set; }
        public List<string> IncludeExtension { get; set; }
        public List<string> ExcludeExtension { get; set; }

        public DirectoryItems(string source, string target)
        {
            Target = target;
            Source = source;

            FileFilter fileFilter = new FileFilter();
            SetCommand setCommand = new SetCommand(fileFilter.Md5Check);
        }

        /// <summary>
        /// The copy files recursively.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="target"> The target. </param>
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                string path = Path.Combine(target.FullName, file.Name);

                if (!File.Exists(path))
                {
                    file.CopyTo(path);
                }
            }
        }

        public void Remove() { }
        public void Move() { }

        private bool Compare()
        {
            return false;
        }
    }

    public class FileFilter 
    {
        public bool Md5Check(FileInfo source)
        {
            return source.Exists;
        }
    }
}
