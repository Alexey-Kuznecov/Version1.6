
namespace UnityCommander.Core.IO.Operations
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;

    using UnityCommander.Native;
    using UnityCommander.Native.Api;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The copy manager.
    /// </summary>
    [ManagerAttach(typeof(CopyManager))]
    public class CopyFiles : IDisposable
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
        /// Used to calculate the average copy file speed.
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
        private static FileOperations fileOperation;

        /// <summary>
        /// The data model for storing information about 
        /// the progress of files copying.
        /// </summary>
        private static CopyInfo copyInfo;

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

        private static double lastByteDone;

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
        public event EventHandler CopyReportEvent;

        #endregion

        #region Test Properties

        /// <summary>
        /// Gets the operations.
        /// </summary>
        public static FileOperations Operations => fileOperation;

        /// <summary>
        /// Gets the copy info instance.
        /// </summary>
        public static CopyInfo CopyInfoInstance => copyInfo;

        /// <summary>
        /// Gets the copy info instance.
        /// </summary>
        public static Timer GetElapsedTimer => ElapsedTimer;

        /// <summary>
        /// Gets the copy info instance.
        /// </summary>
        public static Timer GetSpeedTimer => SpeedTimer;

        #endregion

        public CopyFiles()
        {
            fileOperation = new FileOperations();
            copyInfo = new CopyInfo();
            SpeedTimer.Elapsed += SpeedTimerHandler;
            ElapsedTimer.Elapsed += ElapseTimerHandler;
        }

        /// <summary>
        /// Changes the copy status to stop, resume, or cancel the copy altogether.
        /// </summary>
        /// <param name="changeOn"> Select status to change the copying behavior. </param>
        public void ChangeCopyStatus(CopyBehaviors changeOn)
        {
            copyBehaviors = changeOn;
        }

        public void Copy(string source, string target)
        {
            SpeedTimer.Start();
            ElapsedTimer.Start();
            this.CalculateTotalFilesSize(source);

            foreach (var oldDir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var newDir = oldDir.Replace(source, target);

                Directory.CreateDirectory(newDir);
                this.CopyFile(oldDir, newDir);
            }

            if (Directory.GetFiles(source).Length != 0)
            {
                this.CopyFile(source, target);
            }
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
        public void CopyFile(string oldDir, string newDir)
        {
            foreach (var oldFile in Directory.GetFiles(oldDir))
            {
                FileInfo info = new FileInfo(oldFile);
                string newFile = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);

                fileSize = info.Length;
                copyInfo.Name = info.Name;
                copyInfo.Length = info.Length;
                copyInfo.Source = info.FullName;
                copyInfo.Destination = newFile;

                if (!File.Exists(newFile))
                {
                    fileOperation.XCopy(oldFile, newFile, CopyProgressHandle);
                }
            }
        }

        /// <summary>
        /// Calculates the total size of files on another thread.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public void CalculateTotalFilesSize(string source)
        {
            Task.Factory.StartNew(() =>
            {
                totalFileSize = 0;

                foreach (var path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                {
                    FileInfo info = new FileInfo(path);
                    totalFileSize = Interlocked.Add(ref totalFileSize, info.Length);
                    copyInfo.TotalBytes += info.Length;
                }
            });
        }

        public void Dispose()
        {
            SpeedTimer.Stop();
            ElapsedTimer.Stop();
            lastByteDone = 0;
            lastBytes = 0;
            totalFileSize = 0;
            totalBytesLeft = 0;
            percentCounter = 0;
            averageSpeed = 0;
            fileSize = 0;
        }

        /// <summary>
        /// The raise event.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        private void RaiseEvent(CopyInfo info)
        {
            if (CopyReportEvent != null)
            {
                CopyReportEvent.Invoke(null, new CopyReportEventArg(info));
            }
        }

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
                            copyInfo.Percentage = (double)transferred / total * 100;
                            copyInfo.ByteDone += (double)transferred / totalFileSize;
                            copyInfo.CurrentFileSize = total;

                            // Calculating how many bytes been transferred on the previous iterap.
                            var calibration = transferred - lastBytes;

                            // Measurement of time and total time remaining to copy files.
                            CalculateTimeLeft(calibration);
                            Interlocked.Add(ref currentBytesTransferred, calibration);

                            // Saving an intermediate calculation for the next iteration..
                            lastBytes = transferred;

                            copyInfo.TotalByteDone = lastByteDone + transferred;

                            if (transferred == total)
                            {
                                lastByteDone += transferred;
                            }

                            // Report current copy progress.
                            Interlocked.Exchange(ref copyInfo, copyInfo);
                            RaiseEvent(copyInfo);
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
            copyInfo.TimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));

            // Pass the transferred bytes to the elapsed timer.
            if (byteTransferred > 0)
            {
                totalBytesLeft = Interlocked.Add(ref totalBytesLeft, byteTransferred);
            }
        }

        #endregion

        #region Timer Handlers

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="System.Timers.ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> Expected the <see cref="System.Timers.Timer"/> object. </param>
        /// <param name="e"> Provide data for the <see cref="System.Timers.Timer.Elapsed"/> </param> event.
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
                copyInfo.AverageSpeed = averageSpeed;
            }
        }

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="System.Timers.ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> The event source. </param>
        /// <param name="e"> Expected <see cref="ElapsedEventArgs"/> object, event data. </param>
        private static void ElapseTimerHandler(object sender, ElapsedEventArgs e)
        {
            // Calculate the percentage completed to copy files.
            if (totalBytesLeft > percentCounter)
            {
                percentCounter += totalFileSize / 100;
              
                // TODO: Progress bar not filling 100 percent. Fix it.
                copyInfo.TotalPercentage = (double)totalBytesLeft / totalFileSize * 100;
            }

            // Calculates the time left completed to copy files.
            if (averageSpeed > 0)
            {
                var bps = (totalFileSize - totalBytesLeft) / averageSpeed;
                var timeLeft = TimeSpan.FromSeconds(bps);
                totalTimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
                copyInfo.TotalTimeLeft = totalTimeLeft;
            }
        }

        #endregion
    }
}
