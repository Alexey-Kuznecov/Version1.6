#define NLog

using System;
using System.IO;
using System.Security.AccessControl;
using NLog;

namespace UnityCommander.Core.IO
{
    public delegate bool SetCommand(FileInfo sourceFile);

    public class FileDublicator
    {

#if (NLog)
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif
        /// <summary>
        /// Contains the file permissions and the file owner.
        /// </summary>
        static NTFSAccountModel _accountModel;
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
        public static bool IncludeNTFSRights { get; set; } = true;
        /// <summary>
        /// The event occurs while copying one of the file. 
        /// </summary>
        public static event EventHandler<CopyInfoEventArgs> CopyingEvent;
        /// <summary>
        /// The event occurs when copy start. 
        /// </summary>
        public static event EventHandler<object> CopyStartEvent;
        /// <summary>
        /// The event occurs when copy completed. 
        /// </summary>
        public static event EventHandler<object> CopyCompleteEvent;
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDublicator"/> class.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        public FileDublicator(DirectoryInfo source, DirectoryInfo destination)
        {
            CopyStartEvent += BackupNTFSRights_CopyStartEvent;
            CopyCompleteEvent += RestoreNTFSRights_CopyCompleteEvent;
            this._source = source;
            this._destination = destination;
        }
        /// <summary>
        /// Restores a file permissions and a file owner.
        /// </summary>
        /// <param name="sender"> Initiator class. </param>
        /// <param name="e"> Expected name of the ntfs account. </param>
        private void RestoreNTFSRights_CopyCompleteEvent(object sender, object e)
        {
            if (IncludeNTFSRights)
            {
                NTFSSecurity.AddAccessRuleList((string)e, _accountModel.Accounts);
                NTFSSecurity.TakeOwnership(_accountModel.Owner.Value);
            }
        }
        /// <summary>
        /// Makes a deep copy of the file's permissions 
        /// and writes it to the _accountModel field.
        /// </summary>
        /// <param name="sender"> Initiator class. </param>
        /// <param name="e"> Expected name of the ntfs account. </param>
        private void BackupNTFSRights_CopyStartEvent(object sender, object e)
        {
            if (IncludeNTFSRights)
            {
                _accountModel = new NTFSAccountModel();
                _accountModel.Accounts = NTFSSecurity.GetNTAccounts((string)e);
                _accountModel.Owner = NTFSSecurity.GetOwner((string)e);
            }
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
                    CopyStartEvent(this, file.FullName);
                    this.CopyFileByte(file.FullName, path);
                    CopyCompleteEvent(this, path);
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
                    var byteInPercent = this._fileSizeByte / 101;
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
                    Logger.Error(ex.Message);
                }
            }
        }
        /// <summary>
        /// Restores access to file/folder if this is posible.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        public static bool TryRestoreAccessFile(string path)
        {
            try
            {
                NTFSSecurity.AddNTAccount(path, "Все",
                        FileSystemRights.FullControl,
                        AccessControlType.Allow);
                return true;
            }
            catch (Exception)
            {
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
