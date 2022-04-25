
namespace UnityCommander.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;

    using UnityCommander.Native;
    using UnityCommander.Native.Api;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The file copier reworks.
    /// </summary>
    [ManagerAttach(typeof(FileCopierManager))]
    public class FileCopier : IDisposable
    {
        #region Fields

        /// <summary>
        /// Contains list of the most recently transferred bytes, maximum 30 snapshots.
        /// Used to calculate the average copy file speed.
        /// </summary>
        private readonly Queue<long> snapshots = new Queue<long>();

        /// <summary>
        /// Timer for measuring the average speed of copying a file per second.
        /// </summary>
        private readonly Timer speedTimer = new Timer();

        /// <summary>
        /// Timer to measure the time remaining for copying files, 
        /// also calculates the percentage of file copy progress
        /// </summary>
        private readonly Timer elapsedTimer = new Timer(1000D);

        /// <summary>
        /// A reference to the <see cref="FileOperations"/> class 
        /// which is responsible for copying files.
        /// </summary>
        private readonly FileOperations operations;

        /// <summary>
        /// The data model for storing information about 
        /// the progress of files copying.
        /// </summary>
        private readonly Parameters parameters;

        /// <summary>
        /// The current status to copy files.
        /// </summary>
        private CopyBehaviors copyBehaviors;

        /// <summary>
        /// The current number of bytes transferred.
        /// </summary>
        private long currentBytesTransferred;

        /// <summary>
        /// The size of the file in bytes used to calculate the percentage 
        /// and time remaining to the copy file.
        /// </summary>
        private long fileSize;

        /// <summary>
        /// Total files size used to calculate percentage and time left.
        /// </summary>
        private long totalFileSize;

        /// <summary>
        /// The total number of bytes left to copy files.
        /// </summary>
        private long totalBytesLeft;

        /// <summary>
        /// The last bytes that have been transferred in the <see cref="CopyProgressHandle"/> method.
        /// </summary>
        private long lastBytes;

        /// <summary>
        /// The average speed of the copy files per second.
        /// </summary>
        private double averageSpeed;

        /// <summary>
        /// The total number remaining time to copy files.
        /// </summary>
        private TimeSpan totalTimeLeft;

        /// <summary>
        /// The start speed timer.
        /// </summary>
        private bool startSpeedTimer;

        /// <summary>
        /// Used to measure the total number of bytes copied as a percentage..
        /// </summary>
        private double percentTotalSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCopier"/> class.
        /// </summary>
        public FileCopier()
        {
            this.parameters = new Parameters();
            this.operations = new FileOperations();
            this.speedTimer.Elapsed += this.SpeedTimerHandler;
            this.elapsedTimer.Elapsed += this.ElapseTimerHandler;
        }

        /// <summary>
        /// The copy progress report.
        /// </summary>
        public event EventHandler<Parameters> CopyProgressReport;

        #endregion

        /// <summary>
        /// The flags external control of the copy behavior.
        /// </summary>
        public enum CopyBehaviors : byte
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
        /// Gets the get parameters.
        /// </summary>
        public Parameters GetParameters => this.parameters;

        /// <summary>
        /// Gets or sets the get elapsed timer.
        /// </summary>
        public Timer GetElapsedTimer => this.elapsedTimer;

        /// <summary>
        /// Gets or sets the get speed timer.
        /// </summary>
        public Timer GetSpeedTimer => this.speedTimer;

        #region Public Methods

        /// <summary>
        /// The copy files.
        /// </summary>
        /// <param name="oldDir"> The path to the old directory. </param>
        /// <param name="newDir"> The path to the new directory. </param>
        public void CopyFiles(string oldDir, string newDir)
        {
            foreach (var oldFile in Directory.GetFiles(oldDir))
            {
                FileInfo info = new FileInfo(oldFile);
                string newFile = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);
                this.totalBytesLeft += info.Length;
                this.fileSize = info.Length;
                this.parameters.Name = info.Name;
                this.parameters.Length = info.Length;
                this.parameters.Source = info.FullName;
                this.parameters.Destination = newFile;

                if (!File.Exists(newFile))
                {
                    this.parameters.Skipped = this.operations.XCopy(oldFile, newFile, this.CopyProgressHandle);
                }
            }
        }

        /// <summary>
        /// The copy files.
        /// </summary>
        /// <param name="filePath"> The path to the old directory. </param>
        /// <param name="dirPath"> The path to the new directory. </param>
        public void CopyFile(string filePath, string dirPath)
        {
            FileInfo info = new FileInfo(filePath);
            string newFile = Path.Combine(dirPath, new DirectoryInfo(filePath).Name);
            this.totalBytesLeft += info.Length;
            this.fileSize = info.Length;
            this.parameters.Name = info.Name;
            this.parameters.Length = info.Length;
            this.parameters.Source = info.FullName;
            this.parameters.Destination = newFile;
            this.operations.XCopy(filePath, newFile, this.CopyProgressHandle);
        }

        /// <summary>
        /// Calculates the total size of files on another thread.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public void CalculateTotalFilesSize(string source)
        {
            //Task.Factory.StartNew(() =>
            //{
            //    this.totalFileSize = 0;

            //    foreach (var path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            //    {
            //        FileInfo info = new FileInfo(path);
            //        Interlocked.Add(ref this.totalFileSize, info.Length);
            //    }

            //    this.percentTotalSize = this.totalFileSize > 0 ? this.totalFileSize / 100 : throw new DivideByZeroException();
            //});

            this.totalFileSize = 0;

            foreach (var path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                FileInfo info = new FileInfo(path);
                // Interlocked.Add(ref this.totalFileSize, info.Length);
                this.totalFileSize = info.Length;
            }

            this.percentTotalSize = this.totalFileSize > 0 ? this.totalFileSize / 100 : throw new DivideByZeroException();
        }

        /// <summary>
        /// Changes the copy status to stop, resume, or cancel the copy altogether.
        /// </summary>
        /// <param name="changeOn"> Select status to change the copying behavior. </param>
        public void ChangeCopyStatus(CopyBehaviors changeOn)
        {
            this.copyBehaviors = changeOn;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.speedTimer.Stop();
            this.elapsedTimer.Stop();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// The copy progress handler.
        /// </summary>
        /// <param name="total"> The total. </param>
        /// <param name="transferred"> The transferred. </param>
        /// <param name="streamSize"> The stream size. </param>
        /// <param name="streamByteTrans"> The stream byte trans. </param>
        /// <param name="streamNumber"> The stream number. </param>
        /// <param name="reason"> The reason. </param>
        /// <param name="sourceFile"> The source file. </param>
        /// <param name="destinationFile"> The destination file. </param>
        /// <param name="data"> The data. </param>
        /// <returns> The <see cref="CopyProgressResult"/>. </returns>
        private CopyProgressResult CopyProgressHandle(
            long total,
            long transferred,
            long streamSize,
            long streamByteTrans,
            uint streamNumber,
            CopyProgressCallbackReason reason,
            IntPtr sourceFile,
            IntPtr destinationFile,
            IntPtr data)
        {
            switch (reason)
            {
                case CopyProgressCallbackReason.CALLBACK_CHUNK_FINISHED:
                    switch (this.copyBehaviors)
                    {
                        case CopyBehaviors.Pause:
                            var timeout = TimeSpan.FromMinutes(10D);
                            var mre = new ManualResetEvent(false);
                            while (this.copyBehaviors != CopyBehaviors.Resume)
                            {
                                mre.Set();
                                mre.WaitOne(timeout);
                            }

                            return CopyProgressResult.PROGRESS_CONTINUE;
                        case CopyBehaviors.Cancel:
                            return CopyProgressResult.PROGRESS_CANCEL;
                        default:
                            // Initial the CopyInfo object. 
                            this.parameters.Percentage = (double)transferred / total * 100;
                            this.parameters.ByteDone = transferred;
                            this.parameters.FileSize = total;

                            // Calculating how many bytes been transferred on the previous iteration.
                            var currentBytes = transferred - this.lastBytes > 0 ? transferred - this.lastBytes : transferred;

                            // Measurement of time and total time remaining to copy files.
                            this.CalculateTimeLeft(currentBytes, transferred);
                           
                            Interlocked.Add(ref this.currentBytesTransferred, currentBytes);

                            if (!this.startSpeedTimer)
                                this.GetSpeedTimer.Start();

                            // Saving an intermediate calculation for the next iteration..
                            this.lastBytes = transferred;

                            // Report current copy progress.
                            this.CopyProgressReport?.Invoke(null, this.parameters);

                            return CopyProgressResult.PROGRESS_CONTINUE;
                    }

                default:
                    this.currentBytesTransferred = Interlocked.Add(ref this.currentBytesTransferred, transferred);
                    return CopyProgressResult.PROGRESS_CONTINUE;
            }
        }

        /// <summary>
        /// Calculates time remaining to copy files.  .
        /// </summary>
        /// <param name="currentBytes">
        /// Current bytes transferred.
        /// </param>
        /// <param name="byteTransferred">
        /// Total bytes transferred.
        /// </param>
        private void CalculateTimeLeft(long currentBytes, long byteTransferred)
        {
            var bps = (this.fileSize - this.lastBytes) / currentBytes;
            var timeLeft = TimeSpan.FromSeconds(bps);
            this.parameters.TimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
            this.parameters.TotalPercentage = ((double)this.totalBytesLeft / this.totalFileSize) * 100;
        }

        #endregion

        #region Timer Handlers

        /// <summary>
        /// The bytes transferred.
        /// </summary>
        private long bytesTransferred;

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="System.Timers.ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> Expected the <see cref="System.Timers.Timer"/> object. </param>
        /// <param name="e"> Provide data for the <see cref="System.Timers.Timer.Elapsed"/> </param> event.
        private void SpeedTimerHandler(object sender, ElapsedEventArgs e)
        {
            this.startSpeedTimer = true;
            var elapsedMillisecond = e.SignalTime.Millisecond;
            this.bytesTransferred += Interlocked.Exchange(ref this.currentBytesTransferred, 0L);

            if (elapsedMillisecond > 900)
            {
                if (this.snapshots.Count == 10)
                {
                    this.snapshots.Dequeue();
                }

                this.snapshots.Enqueue(this.bytesTransferred);
                Interlocked.Exchange(ref this.averageSpeed, this.snapshots.Average());

                if (this.averageSpeed > 0)
                {
                    this.parameters.AverageSpeed = this.averageSpeed;
                }

                this.bytesTransferred = 0;
                this.startSpeedTimer = false;
                this.GetSpeedTimer.Stop();
            }
        }

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="System.Timers.ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> The event source. </param>
        /// <param name="e"> Expected <see cref="ElapsedEventArgs"/> object, event data. </param>
        private void ElapseTimerHandler(object sender, ElapsedEventArgs e)
        {
            // Calculates the time left completed to copy files.
            if (this.averageSpeed > 0)
            {
                var bps = (this.totalFileSize - this.totalBytesLeft) / this.averageSpeed;
                var timeLeft = TimeSpan.FromSeconds(bps);
                this.totalTimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
                this.parameters.TotalTimeLeft = this.totalTimeLeft;
            }
        }

        #endregion

        /// <summary>
        /// The parameters.
        /// </summary>
        public class Parameters
        {
            /// <summary>
            /// Gets or sets the name file.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the source directory.
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// Gets or sets the target directory.
            /// </summary>
            public string Destination { get; set; }

            /// <summary>
            /// Gets or sets the file length in bytes.
            /// </summary>
            public long Length { get; set; }

            /// <summary>
            /// Gets or sets the average file copy rate per second.
            /// </summary>
            public double AverageSpeed { get; set; }

            /// <summary>
            /// Gets or sets a value indicating the time remaining for copying the file.
            /// </summary>
            public TimeSpan TimeLeft { get; set; }

            /// <summary>
            /// Gets or sets the value indicating the time remaining for copying all files.
            /// </summary>
            public TimeSpan TotalTimeLeft { get; set; }

            /// <summary>
            /// Gets or sets a value indicating the percentage of file copy progress.
            /// </summary>
            public double Percentage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating the percentage of files copy progress.
            /// </summary>
            public double TotalPercentage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating the copied bytes.
            /// </summary>
            public double ByteDone { get; set; }

            /// <summary>
            /// Gets or sets the value indicating file size.
            /// </summary>
            public double FileSize { get; set; }

            /// <summary>
            /// Gets or sets the value indicating total size of files.
            /// </summary>
            public double TotalFileSize { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether there are copy errors.
            /// </summary>
            public bool Skipped { get; set; }
        }
    }
}