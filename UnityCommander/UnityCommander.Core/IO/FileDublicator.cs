
#define NLog

namespace UnityCommander.Core.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Security.AccessControl;
    using System.Threading;
    using NLog;

    /// <summary>
    /// The flags external control of the copy behavior.
    /// </summary>
    public enum Status : byte
    {
        /// <summary>
        /// Copy is started.
        /// </summary>
        Resume = 0,

        /// <summary>
        /// Copy is paused.
        /// </summary>
        Pause = 1,

        /// <summary>
        /// Copy is canceled.
        /// </summary>
        Cancel = 2
    }

    /// <summary>
    /// The file duplicator.
    /// </summary>
    public class FileDublicator
    {
        #region Declaration Field

#if (NLog)
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif
        /// <summary>
        /// External control status of the copy process
        /// </summary>
        private static Status status;

        /// <summary>
        /// Propagate notification that copy operation should be canceled.
        /// </summary>
        private static CancellationToken cancellationToken;

        /// <summary>
        /// Contains the file permissions and the file owner.
        /// </summary>
        private static NTFSAccountModel accountModel;

        /// <summary>
        /// Contains information about a source directory. 
        /// </summary>
        private readonly DirectoryInfo sourceInfo;

        /// <summary>
        /// Contains information about a target directory.
        /// </summary>
        private readonly DirectoryInfo destinationInfo;

        /// <summary>
        /// Contains the size of the file in bytes.  
        /// </summary>
        private long fileSizeByte;

        /// <summary>
        /// Contains the file copy completion percentage,
        /// which determines the progress bar step.
        /// </summary>
        private int fileCopyIndicator;

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDublicator"/> class.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        public FileDublicator(DirectoryInfo source, DirectoryInfo destination)
        {
            CopyStartEvent += BackupNtfsRightsCopyStartEvent;
            CopyCompleteEvent += RestoreNtfsRightsCopyStopEvent;
            this.sourceInfo = source;
            this.destinationInfo = destination;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDublicator"/> class.
        /// </summary>
        public FileDublicator()
        {
        }

        #endregion

        #region Declaration Events

        /// <summary>
        /// The event occurs when copying one of the file. 
        /// </summary>
        public static event EventHandler<CopyInfoEventArgs> CopyingEvent;

        /// <summary>
        /// The event occurs at the beginning of file copying. 
        /// </summary>
        private static event EventHandler<object> CopyStartEvent;

        /// <summary>
        /// The event occurs when the file has been completely copied. 
        /// </summary>
        private static event EventHandler<object> CopyCompleteEvent;

        /// <summary>
        /// The event occurs when cancellation copying files. 
        /// </summary>
        private static event EventHandler<object> CopyCancelEvent;

        /// <summary>
        /// The event occurs when the user pauses copying files. 
        /// </summary>
        private static event EventHandler<object> CopyPauseEvent;

        /// <summary>
        /// The event occurs when the user starts copying files after a temporary stop.
        /// </summary>
        private static event EventHandler<object> CopyResumeEvent;

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether to copy whether file/folder ntfs rights.
        /// </summary>
        public static bool IncludeNTFSRights { get; set; }

        #region Event Invocators

        /// <summary>
        /// Raise copy start event.
        /// </summary>
        /// <param name="e"> The e. </param>
        public static void OnCopyStartEvent(object e)
        {
            CopyStartEvent?.Invoke(null, e);
        }

        /// <summary>
        /// Raise copy complete event.
        /// </summary>
        /// <param name="e"> The e. </param>
        public static void OnCopyCompleteEvent(object e)
        {
            CopyCompleteEvent?.Invoke(null, e);
        }

        /// <summary>
        /// Raise copy cancel event
        /// </summary>
        /// <param name="e"> The e. </param>
        public static void OnCopyCancelEvent(object e)
        {
            CopyCancelEvent?.Invoke(null, e);
        }

        /// <summary>
        /// Raise copy pause event.
        /// </summary>
        /// <param name="e"> The e. </param>
        public static void OnCopyPauseEvent(object e)
        {
            CopyPauseEvent?.Invoke(null, e);
        }

        /// <summary>
        /// Raise copy resume event.
        /// </summary>
        /// <param name="e"> The e. </param>
        public static void OnCopyResumeEvent(object e)
        {
            CopyResumeEvent?.Invoke(null, e);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The calc total size file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public long CalcTotalSizeFile(string path)
        {
            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(path);
            }

            return 0;
        }

        /// <summary>
        /// This method can stop, resume, or cancel the copy altogether.
        /// </summary>
        /// <param name="changeOn"> Select status to change copy behavior. </param>
        public static void ChangeCopyStatus(Status changeOn)
        {
            status = changeOn;
        }

        /// <summary>
        /// Recursively navigates over directories. Creates a directory structure at the destination 
        /// and discovers each file at the source. If a file was found then passes it in 
        /// arguments to another method to copy it to the destination.
        /// </summary>
        /// <param name="cancelToken"> The token to cancel the copy operation. </param>
        /// <returns> True if the copy operation was successful. </returns>
        public bool CreateDirectoryStructure(CancellationToken cancelToken)
        {
            cancellationToken = cancelToken;

            foreach (string dirPath in Directory.GetDirectories(this.sourceInfo.FullName, "*", SearchOption.AllDirectories))
            {
                var newDir = dirPath.Replace(this.sourceInfo.FullName, this.destinationInfo.FullName);

                Directory.CreateDirectory(newDir);

                foreach (var oldFile in Directory.GetFiles(dirPath))
                {
                    string currentFileTarget = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);

                    if (!File.Exists(currentFileTarget))
                    {
                        this.CopyFileByte(oldFile, currentFileTarget);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Copies the file by byte using stream of the files.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        [SuppressMessage("ReSharper", "StyleCop.SA1503")]
        public void CopyFileByte(string source, string destination)
        {
            FileStream inFileStream = this.SafetyOpenFile(source, destination);
            using (var outFileStream = new FileStream(destination, FileMode.Create))
            {
                this.fileCopyIndicator = 0;
                this.fileSizeByte = inFileStream.Length;
                int count = 0, b;
                var byteInPercent = this.fileSizeByte / 101;

                // This part of the code responsible for copying files. 
                while ((b = inFileStream.ReadByte()) >= 0)
                {
                    outFileStream.WriteByte((byte)b);
                    count++;

                    // Looping simulates pausing the copy process.
                    if (status == Status.Pause)
                        while (status != Status.Resume)

                    // Throw exception for cancel the copy process
                    if (status == Status.Cancel)
                        cancellationToken.ThrowIfCancellationRequested();

                    // This part of the code serves to fix the moment at 
                    // which you can see the progress of copying the file.
                    if (count <= byteInPercent) continue;

                    // Sending useful information to each subscribers.
                    // For example, report on the copying file.
                    if (CopyingEvent != null)
                    {
                        this.ExportProgressBarReport(source, destination);
                    }

                    count = 0;
                }

                inFileStream.Close();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Restores a file permissions and a file owner.
        /// </summary>
        /// <param name="sender"> Initiator class. </param>
        /// <param name="path"> Expected the path to the file. </param>
        private static void RestoreNtfsRightsCopyStopEvent(object sender, object path)
        {
            if (IncludeNTFSRights)
            {
                NTFSSecurity.AddAccessRuleList((string)path, accountModel.Accounts);
            }
        }

        /// <summary>
        /// Makes a deep copy of the file's permissions 
        /// and writes it to the accountModel field.
        /// </summary>
        /// <param name="sender"> Initiator class. </param>
        /// <param name="e"> Expected the path to the file. </param>
        private static void BackupNtfsRightsCopyStartEvent(object sender, object e)
        {
            if (IncludeNTFSRights)
            {
                string path = (string)e;
                accountModel = new NTFSAccountModel
                {
                    Accounts = NTFSSecurity.GetNTAccounts(path),
                    Owner = NTFSSecurity.GetOwner(path)
                };
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Safely opens the file if more permissions are required to open the file.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="destination"> The destination. </param>
        /// <returns> The <see cref="FileStream"/>. </returns>
        private FileStream SafetyOpenFile(string source, string destination)
        {
            try
            {
                return new FileStream(source, FileMode.Open);
            }
            catch (Exception ex)
            {
                if (this.TryRestoreAccessFile(source))
                {
                    this.CopyFileByte(source, destination);
                }
                else
                {
                    this.ExportProgressBarReport(source, destination, ex.Message);
                    Logger.Error(ex.Message);
                }

                return new FileStream(source, FileMode.Open);
            }
        }

        /// <summary>
        /// Restores access to file/folder if this is possible.
        /// </summary>
        /// <param name="path"> The path to file/folder. </param>
        /// <returns> True if access to the file was restored. </returns>
        private bool TryRestoreAccessFile(string path)
        {
            try
            {
                NTFSSecurity.AddNTAccount(path, "Все", FileSystemRights.FullControl, AccessControlType.Allow);
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
        /// <param name="source"> The source of the file.  </param>
        /// <param name="destination"> The destination of the file. </param>
        /// <param name="error"> The errors. </param>
        private void ExportProgressBarReport(string source, string destination, string error = null)
        {
            // Generates a report on the copied file. 
            CopyInfoModel copyInfo = new CopyInfoModel
            {
                Source = source,
                Destination = destination,
                FileLength = this.fileSizeByte,
                ProgressBar = ++this.fileCopyIndicator,
                ErrorMessage = error
            };

            // Raise event for a notify subscribers.
            CopyingEvent?.Invoke(this, new CopyInfoEventArgs(copyInfo));
        }

        #endregion
    }
}
