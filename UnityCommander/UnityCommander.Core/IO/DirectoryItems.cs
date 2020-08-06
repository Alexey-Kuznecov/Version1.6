using System;
using System.IO;
using System.Windows;

namespace UnityCommander.Core.IO
{
    public delegate bool SetCommand(FileInfo sourceFile);

    public class DirectoryItems
    {
        private DirectoryInfo _source;
        private DirectoryInfo _target;
        private bool _askFileReplace;
        private DirectoryInfo _currentCopy;

        /// <summary>
        /// The event occurs while copying one of the file. 
        /// </summary>
        public static event EventHandler<CopyInfoEventArgs> CopyingEvent;
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryItems"/> class.
        /// </summary>
        public DirectoryItems(DirectoryInfo source, DirectoryInfo target)
        {
            this._source = source;
            this._target = target;
            //FileFilter fileFilter = new FileFilter();
            //SetCommand setCommand = new SetCommand(fileFilter.Md5Check);
        }
       
        public void Copy()
        {
            this.CopyFilesRecursively(this._source, this._target);
            //this.CopyFiles();         
        }

        public void CopyFiles()
        {
            foreach (string dirPath in Directory.GetDirectories(this._source.FullName, "*",
               SearchOption.AllDirectories))
            {
                try
                {
                    Directory.CreateDirectory(dirPath.Replace(this._source.FullName, this._target.FullName));
                }
                catch (Exception e)
                {
                    //здесь обрабатывай ошибки
                }
            }

            foreach (var file in Directory.GetFiles(this._source.FullName, "*.*",
               SearchOption.AllDirectories))
            {
                try
                {
                    // string path = Path.Combine(this._target.FullName, file.);
                    File.Copy(file, file.Replace(this._source.FullName, this._target.FullName), true);

                }
                catch (Exception e)
                {
                    //здесь обрабатывай ошибки
                }
            }
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

                    if (CopyingEvent != null)
                    {
                        // Generates a report on the copied file. 
                        CopyInfoModel copyInfo = new CopyInfoModel
                        {
                            SourceFile = file.FullName,
                            TargetFile = _target.FullName
                        };
                        // Raise event for a notify subscribers.
                        CopyingEvent(this, new CopyInfoEventArgs(copyInfo));
                    }
                }
            }
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
