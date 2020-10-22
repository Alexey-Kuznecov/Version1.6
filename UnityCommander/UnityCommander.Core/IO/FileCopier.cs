
#define Nlog

namespace UnityCommander.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;

    using NLog;

    using UnityCommander.Native;
    using UnityCommander.Native.Api;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// Provides copying files interface.
    /// </summary>
    public class FileCopier
    {
        #region Fields

#if (Nlog)
        /// <summary>
        /// The reference the current log event manager.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
#endif
        /// <summary>
        /// Contains list of the most recently transferred bytes, maximum 30 snapshots.
        /// Used to culculate the average copy file speed.
        /// </summary>
        private static readonly Queue<long> Snapshots = new Queue<long>(30);

        /// <summary>
        /// Timer for measuring the average speed of copying a file per second.
        /// </summary>
        private static readonly Timer SpeedTimer = new Timer(1000D);

        /// <summary>
        /// Timer to measure the time remaining for copying files, 
        /// also calculates the percentage of file copy progress
        /// </summary>
        private static readonly Timer ElapsedTimer = new Timer(1000D);

        /// <summary>
        /// The current status to copy files.
        /// </summary>
        private static CopyBehaviors copyBehaviors;

        /// <summary>
        /// A reference to the <see cref="FileOperations"/> class 
        /// which is responsible for copying files.
        /// </summary>
        private static FileOperations operations;

        /// <summary>
        /// The data model for storing information about 
        /// the progress of files copying.
        /// </summary>
        private static CopyInfo copyInfoInstance;

        /// <summary>
        /// The current number of bytes transferred.
        /// </summary>
        private static long currentBytesTransferred;

        /// <summary>
        /// The size of the file in bytes used to calculate the percentage 
        /// and time remaining to the copy file.
        /// </summary>
        private static long fileSize;

        /// <summary>
        /// The bytes left to the copy file.
        /// </summary>
        private static long bytesLeft;

        /// <summary>
        /// Total files size used to calculate percentage and time left.
        /// </summary>
        private static long totalFileSize;

        /// <summary>
        /// The total number of bytes left to copy files.
        /// </summary>
        private static long totalBytesLeft;

        /// <summary>
        /// The last bytes that have been transferred in the <see cref="CopyProgressHandle"/> method.
        /// </summary>
        private static long lastBytes;

        /// <summary>
        /// The average speed of the copy files per second.
        /// </summary>
        private static double averageSpeed;

        /// <summary>
        /// The total number remaining time to copy files.
        /// </summary>
        private static TimeSpan totalTimeLeft;

        /// <summary>
        /// Used to measure the total number of bytes copied as a percentage..
        /// </summary>
        private static double percentCounter;

        /// <summary>
        /// The copy progress report.
        /// </summary>
        public static event EventHandler<CopyInfo> CopyProgressReport;

        /// <summary>
        /// The copy file result.
        /// </summary>
        public static event EventHandler<CopyInfo> CopyFileResult;

        #endregion

        #region Enums

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Changes the copy status to stop, resume, or cancel the copy altogether.
        /// </summary>
        /// <param name="changeOn"> Select status to change the copying behavior. </param>
        public static void ChangeCopyStatus(CopyBehaviors changeOn)
        {
            copyBehaviors = changeOn;
        }

        /// <summary>
        /// Copies all files from source to destination.
        /// </summary>
        /// <param name="src"> The part to source files. </param>
        /// <param name="dest"> The part to destination. </param>
        public static void StartCopyDeep(string src, string dest)
        {
            Task.Factory.StartNew(() =>
            {
                operations = new FileOperations();
                copyInfoInstance = new CopyInfo();
                SpeedTimer.Elapsed += SpeedTimerHandler;
                ElapsedTimer.Elapsed += ElapseTimerHandler;
                CalculateTotalFilesSize(src);

                SpeedTimer.Start();
                ElapsedTimer.Start();

                foreach (var oldDir in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
                {
                    var newDir = oldDir.Replace(src, dest);
                    Directory.CreateDirectory(newDir);
                    CopyFiles(oldDir, newDir);
                    CopyFileResult?.Invoke(null, copyInfoInstance);
                }

                // Also copy files in the root directory of the source.
                if (Directory.GetFiles(src).Length != 0)
                {
                    CopyFiles(src, dest);
                }

                SpeedTimer.Stop();
                ElapsedTimer.Stop();
            });
        }

        /// <summary>
        /// The copy files.
        /// </summary>
        /// <param name="oldDir">
        /// The path to the old directory.
        /// </param>
        /// <param name="newDir">
        /// The path to the new directory.
        /// </param>
        public static void CopyFiles(string oldDir, string newDir)
        {
            foreach (var oldFile in Directory.GetFiles(oldDir))
            {
                FileInfo info = new FileInfo(oldFile);
                string newFile = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);

                fileSize = info.Length;
                copyInfoInstance.Name = info.Name;
                copyInfoInstance.Length = info.Length;
                copyInfoInstance.Source = info.FullName;
                copyInfoInstance.Destination = newFile;

                if (!File.Exists(newFile))
                {
                    copyInfoInstance.Skipped = operations.XCopy(oldFile, newFile, CopyProgressHandle);
                }
            }
        }

        /// <summary>
        /// Calculates the total size of files on another thread.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void CalculateTotalFilesSize(string source)
        {
            Task.Factory.StartNew(() =>
            {
                totalFileSize = 0;

                foreach (var path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                {
                    FileInfo info = new FileInfo(path);
                    totalFileSize = Interlocked.Add(ref totalFileSize, info.Length);
                }
            });
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
        private static CopyProgressResult CopyProgressHandle(
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
                    switch (copyBehaviors)
                    {
                        case CopyBehaviors.Pause:
                            var timeout = TimeSpan.FromMinutes(10D);
                            var mre = new ManualResetEvent(false);
                            while (copyBehaviors != CopyBehaviors.Resume)
                            {
                                mre.Set();
                                mre.WaitOne(timeout);
                            }

                            return CopyProgressResult.PROGRESS_CONTINUE;
                        case CopyBehaviors.Cancel:
                            return CopyProgressResult.PROGRESS_CANCEL;
                        default:
                            // Initial the CopyInfo object. 
                            copyInfoInstance.Percentage = (double)transferred / total * 100;
                            copyInfoInstance.ByteDone = transferred;
                            copyInfoInstance.FileSize = total;

                            // Calculating how many bytes been transferred on the previous iterap.
                            var calibration = transferred - lastBytes;

                            // Measurement of time and total time remaining to copy files.
                            CalculateTimeLeft(calibration);
                            Interlocked.Add(ref currentBytesTransferred, calibration);

                            // Saving an intermediate calculation for the next iteration..
                            lastBytes = transferred;

                            // Report current copy progress.
                            Interlocked.Exchange(ref copyInfoInstance, copyInfoInstance);
                            CopyProgressReport?.Invoke(null, copyInfoInstance);

                            return CopyProgressResult.PROGRESS_CONTINUE;
                    }

                default:
                    currentBytesTransferred = Interlocked.Add(ref currentBytesTransferred, transferred);
                    return CopyProgressResult.PROGRESS_CONTINUE;
            }
        }

        /// <summary>
        /// Calculates time remaining to copy files.  .
        /// </summary>
        /// <param name="byteTransferred">
        /// Total bytes transferred.
        /// </param>
        private static void CalculateTimeLeft(long byteTransferred)
        {
            var bps = (fileSize - lastBytes) / byteTransferred;
            var timeLeft = TimeSpan.FromSeconds(bps);
            copyInfoInstance.TimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));

            // Pass the transferred bytes to the elapsed timer.
            if (byteTransferred > 0)
            {
                totalBytesLeft = Interlocked.Add(ref totalBytesLeft, byteTransferred);
            }
        }

        #endregion

        #region Timer Handlers

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> Expected the <see cref="Timer"/> object. </param>
        /// <param name="e"> Provide data for the <see cref="Timer.Elapsed"/> </param> event.
        private static void SpeedTimerHandler(object sender, ElapsedEventArgs e)
        {
            var bytesTransferred = Interlocked.Exchange(ref currentBytesTransferred, 0L);

            if (fileSize < 10000000)
            {
                bytesTransferred = bytesTransferred << 2;
            }

            if (bytesTransferred > 0)
            {
                if (Snapshots.Count == 30)
                {
                    Snapshots.Dequeue();
                }

                Snapshots.Enqueue(bytesTransferred);
                Interlocked.Exchange(ref averageSpeed, Snapshots.Average());
                copyInfoInstance.AverageSpeed = averageSpeed;
            }
        }

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> The event source. </param>
        /// <param name="e"> Expacted <see cref="ElapsedEventArgs"/> object, event data. </param>
        private static void ElapseTimerHandler(object sender, ElapsedEventArgs e)
        {
            // Calculate the percentage completed to copy files.
            if (totalBytesLeft > percentCounter)
            {
                percentCounter += totalFileSize / 100;
                copyInfoInstance.TotalPercentage++;
            }

            // Calculates the time left completed to copy files.
            if (averageSpeed > 0)
            {
                var bps = (totalFileSize - totalBytesLeft) / averageSpeed;
                var timeLeft = TimeSpan.FromSeconds(bps);
                totalTimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
                copyInfoInstance.TotalTimeLeft = totalTimeLeft;
            }
        }

        #endregion

        #region Inner Class

        /// <summary>
        /// Provide details on copying files.
        /// </summary>
        public class CopyInfo
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

        #endregion
    }
}
