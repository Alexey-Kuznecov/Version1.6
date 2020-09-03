using System;
using System.IO;
using System.Security.AccessControl;

namespace UnityCommander.Core.IO
{
    public delegate bool SetCommand(FileInfo sourceFile);

    public class FileDublicator
    {
        /// <summary>
        /// Contains information about a source directory. 
        /// </summary>
        private DirectoryInfo _source;
        /// <summary>
        /// Contains information about a target directory.
        /// </summary>
        private DirectoryInfo _destination;
        /// <summary>
        /// Contains the size of the file in bytes.  
        /// </summary>
        private long _fileSizeByte = 0;
        /// <summary>
        /// Contains the file copy completion percentage,
        /// which determines the progress bar step.
        /// </summary>
        private int _fileCopyIndicator = 0;
        /// <summary>
        /// Gets or sets value to copy whether file/folder ntfs rights.
        /// </summary>
        public static bool IncludeNTFSRights { get; set; }
        /// <summary>
        /// The event occurs while copying one of the file. 
        /// </summary>
        public static event EventHandler<CopyInfoEventArgs> CopyingEvent;
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDublicator"/> class.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        public FileDublicator(DirectoryInfo source, DirectoryInfo destination)
        {
            this._source = source;
            this._destination = destination;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDublicator"/> class.
        /// </summary>
        public FileDublicator()
        {
        }
        /// <summary>
        /// Copies all files from source to destination.
        /// </summary>
        public void Copy()
        {
            this.CreateDirectoryStructure(this._source, this._destination);        
        }
        /// <summary>
        /// Recursively navigates over directories. Creates a directory structure at the destination 
        /// and discovers each file at the source. If a file was found then passes it in 
        /// arguments to another method to copy it to the destination.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        public void CreateDirectoryStructure(DirectoryInfo source, DirectoryInfo destination)
        {
            foreach (DirectoryInfo directory in source.GetDirectories())
            {
                CreateDirectoryStructure(directory, destination.CreateSubdirectory(directory.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                string path = Path.Combine(destination.FullName, file.Name);

                if (!File.Exists(path))
                {
                    this.CopyFileByte(file.FullName, path);
                }
            }
        }
        /// <summary>
        /// Copies the file by byte using stream of the files.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        public void CopyFileByte(string source, string destination)
        {
            int count = 0, b = 0;
            this._fileCopyIndicator = 0;
            try
            {
                using (var inFileStream = new FileStream(source, FileMode.Open))
                using (var outFileStream = new FileStream(destination, FileMode.Create))
                {
                    this._fileSizeByte = inFileStream.Length;
                    // Gets a percentage of the file size and subtracts another 100. 
                    // This is how I fixed a boolean error that affects the progress bar result.
                    // Before the fix, the progress bar worked at 99%.
                    var byteInPercent = this._fileSizeByte / 100 - 10;
                    while ((b = inFileStream.ReadByte()) >= 0)
                    {
                        outFileStream.WriteByte((byte)b);
                        count++;
                        // This part of the code serves to fix the moment at 
                        // which you can see the progress of copying the file.
                        if (count > byteInPercent)
                        {
                            count = 0;
                            // Sending useful information to each subscribers.
                            // For example, report on the copying file.
                            if (CopyingEvent != null)
                            {
                                this.ExportProgressBarReport(source, destination);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (TryRestoreAccessFile(source))
                {
                    CopyFileByte(source, destination);
                }
                else
                {
                    this.ExportProgressBarReport(source, destination, ex.Message);
                }
            }
        }
        /// <summary>
        /// Restores access to file/folder.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        public static bool TryRestoreAccessFile(string path)
        {
            var ntAccount = NTFSSecurity.GetNTAccounts(path);
            try
            {
                foreach (var item in ntAccount)
                {
                    if (IncludeNTFSRights)
                    {
                        NTFSSecurity.ChangeAccessRight(path, item.IdentityReference.Value,
                                                                    FileSystemRights.FullControl,
                                                                    AccessControlType.Allow);
                    }
                    else
                    {
                        
                    }
                }
                    
                return true;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// Exports report on a progress bar of the copied file.
        /// </summary>
        /// <param name="source"> The source of the file. </param>
        /// <param name="destination"> The destination of the file. </param>
        private void ExportProgressBarReport(string source, string destination, string error = null)
        {
            // Generates a report on the copied file. 
            CopyInfoModel copyInfo = new CopyInfoModel
            {
                Source = source,
                Destination = destination,
                FileLength = this._fileSizeByte,
                ProgressBar = ++this._fileCopyIndicator,
                ErrorMessage = error
            };
            // Raise event for a notify subscribers.
            CopyingEvent(this, new CopyInfoEventArgs(copyInfo));
        }
    }
}
