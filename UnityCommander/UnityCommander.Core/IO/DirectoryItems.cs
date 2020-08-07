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
        }
        public void Copy()
        {
            this.CopyFilesRecursively(this._source, this._target);
            //this.CopyFiles();         
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
                    this.CopyFileByte(file.FullName, path);
                }
            }
        }

        private void CopyFileByte(string src, string dist)
        {
            int count = 0, perc = 0, b = 0;

            using (var inFileStream = new FileStream(src, FileMode.Open))
            using (var outFileStream = new FileStream(dist, FileMode.Create))
            {
                var length = inFileStream.Length;
                var byteInPerc = length / 100 - 100;
                while ((b = inFileStream.ReadByte()) >= 0)
                {
                    outFileStream.WriteByte((byte)b);
                    count++;
                    if (count > byteInPerc)
                    {
                        count = 0;
                        if (CopyingEvent != null)
                        {
                            // Generates a report on the copied file. 
                            CopyInfoModel copyInfo = new CopyInfoModel
                            {
                                SourceFile = src,
                                TargetFile = dist,
                                FileLength = length,
                                CopyStatus = ++perc
                            };
                            // Raise event for a notify subscribers.
                            CopyingEvent(this, new CopyInfoEventArgs(copyInfo));
                        } 
                    }
                }
            }
        }

        private void ExportReport()
        {

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
