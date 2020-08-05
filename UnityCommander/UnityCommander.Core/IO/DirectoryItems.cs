using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using UnityCommander.Business;

namespace UnityCommander.Core.IO
{
    public delegate bool SetCommand(FileInfo sourceFile);

    public class DirectoryItems
    {
        public DirectoryItems()
        {
            FileFilter fileFilter = new FileFilter();
            SetCommand setCommand = new SetCommand(fileFilter.Md5Check);
        }

        /// <summary>
        /// The copy files recursively.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="target"> The target. </param>
        public void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
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
                    FileCopyInfoModel copyInfo = new FileCopyInfoModel
                    {
                        SourceFile = file.FullName,
                        TargetFile = target.FullName
                    };
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
