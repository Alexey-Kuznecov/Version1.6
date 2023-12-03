#define Nlog

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

namespace UnityCommander.Core.IO.Operations
{
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
        private readonly Queue<long> Snapshots = new Queue<long>(30);

        /// <summary>
        /// Timer for measuring the average speed of copying a file per second.
        /// </summary>
        private readonly Timer SpeedTimer = new Timer(1000D);

        /// <summary>
        /// Timer to measure the time remaining for copying files, 
        /// also calculates the percentage of file copy progress
        /// </summary>
        private readonly Timer ElapsedTimer = new Timer(1000D);

        /// <summary>
        /// The current status to copy files.
        /// </summary>
        private CopyBehaviors copyBehaviors;

        /// <summary>
        /// A reference to the <see cref="FileOperations"/> class 
        /// which is responsible for copying files.
        /// </summary>
        private FileOperations fileOperation;

        /// <summary>
        /// The data model for storing information about 
        /// the progress of files copying.
        /// </summary>
        private CopyInfo copyInfo;

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

        private double lastByteDone;

        /// <summary>
        /// The average speed of the copy files per second.
        /// </summary>
        private double averageSpeed;

        /// <summary>
        /// The total number remaining time to copy files.
        /// </summary>
        private TimeSpan totalTimeLeft;

        /// <summary>
        /// The copy progress report.
        /// </summary>
        public event EventHandler CopyReportEvent;

        /// <summary>
        /// The copy progress report.
        /// </summary>
        public event EventHandler FileAlreadyExistsEvent;

        #endregion

        #region Test Properties

        /// <summary>
        /// Gets the operations.
        /// </summary>
        public FileOperations Operations => fileOperation;

        /// <summary>
        /// Gets the copy info instance.
        /// </summary>
        public CopyInfo CopyInfoInstance => copyInfo;

        /// <summary>
        /// Gets the copy info instance.
        /// </summary>
        public Timer GetElapsedTimer => ElapsedTimer;

        /// <summary>
        /// Gets the copy info instance.
        /// </summary>
        public Timer GetSpeedTimer => SpeedTimer;

        #endregion

        public CopyFiles()
        {
            fileOperation = new FileOperations();
            copyInfo = new CopyInfo();
            SpeedTimer.Elapsed += SpeedTimerHandler;
            ElapsedTimer.Elapsed += ElapseTimerHandler;
        }

        public void Dispose()
        {
            SpeedTimer.Stop();
            ElapsedTimer.Stop();
        }

        /// <summary>
        /// Changes the copy status to stop, resume, or cancel the copy altogether.
        /// </summary>
        /// <param name="changeOn"> Select status to change the copying behavior. </param>
        public void ChangeCopyStatus(CopyBehaviors changeOn)
        {
            copyBehaviors = changeOn;
        }

        internal void RaiseFileAlreadyExistsEvent(CopyInfo info)
        {
            if (this.FileAlreadyExistsEvent != null)
            {
                this.FileAlreadyExistsEvent.Invoke(this, new CopyReportEventArg(info));
            }
        }

        public void Copy(string source, string target)
        {
            FileInfo info = new FileInfo(source);
            totalFileSize = info.Length;
            copyInfo.TotalBytes += info.Length;

            var existPath = File.Exists(source.Replace(new FileInfo(source).Directory.FullName, target));

            if (!existPath)
            {
                SpeedTimer.Start();
                ElapsedTimer.Start();
            }
            
            this.CopyFile(source, target);
        }

        public void DeepCopy(string source, string target)
        {
            this.CalculateTotalFilesSize(source);
            SpeedTimer.Start();
            ElapsedTimer.Start();

            foreach (var oldDir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                var newDir = oldDir.Replace(source, target);

                Directory.CreateDirectory(newDir);

                foreach (var oldFile in Directory.GetFiles(oldDir))
                {
                    this.CopyFile(oldFile, newDir);
                }
            }

            // Копирует файлы которые лежат внутри копируемой папки 
            if (Directory.GetFiles(source).Length != 0)
            {
                foreach (var oldFile in Directory.GetFiles(source))
                {
                    this.CopyFile(oldFile, target);
                }
            }
        }

        /// <summary>
        /// The copy files.
        /// </summary>
        /// <param name="oldFile">
        /// The path to the old directory.
        /// </param>
        /// <param name="newDir">
        /// The path to the new directory.
        /// </param>
        public void CopyFile(string oldFile, string newDir)
        {
            FileInfo info = new FileInfo(oldFile);
            string newFile = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);
            FileInfo infoF = new FileInfo(newFile);

            fileSize = info.Length;
            copyInfo.Name = info.Name;
            copyInfo.Length = info.Length;
            copyInfo.Source = info.FullName;
            copyInfo.Destination = newFile;
            copyInfo.FileInfo = infoF;

            if (File.Exists(copyInfo.Destination))
            {
                this.copyBehaviors = CopyBehaviors.Pause;
                RaiseFileAlreadyExistsEvent(copyInfo);
            }

            fileOperation.XCopy(oldFile, newFile, CopyProgressHandle);
        }

        /// <summary>
        /// Calculates the total size of files on another thread.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public void CalculateTotalFilesSize(string source)
        {
           var t = Task.Factory.StartNew(() =>
            {
                totalFileSize = 0;

                foreach (var path in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
                {
                    FileInfo info = new FileInfo(path);
                    totalFileSize = Interlocked.Add(ref totalFileSize, info.Length);
                    copyInfo.TotalBytes += info.Length;
                }
            });

            t.Wait();
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
                            //copyInfo.ByteDone += (double)transferred / totalFileSize;
                           
                            copyInfo.CurrentFileSize = total;

                            // Calculating how many bytes been transferred on the previous iterap.
                            var calibration = transferred - lastBytes;
                            copyInfo.CurrentBytesTransferred = calibration;

                            // Measurement of time and total time remaining to copy files.
                            CalculateTimeLeft(calibration);
                            Interlocked.Add(ref currentBytesTransferred, calibration);

                            // Saving an intermediate calculation for the next iteration..
                            lastBytes = transferred;

                            copyInfo.TotalByteDone = lastByteDone + transferred;
                            copyInfo.TotalPercentage = copyInfo.TotalByteDone / totalFileSize * 100;   
                            
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
        private void CalculateTimeLeft(long byteTransferred)
        {
            if (byteTransferred > 0)
            {
                var bps = (fileSize - lastBytes) / byteTransferred;
                var timeLeft = TimeSpan.FromSeconds(bps);
                copyInfo.TimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));

                // Pass the transferred bytes to the elapsed timer.
                totalBytesLeft += byteTransferred;
                copyInfo.TotalBytesTransferred = totalBytesLeft;
            }
        }

        #endregion

        #region Timer Handlers

        /// <summary>
        /// Calculates the average speed when occurring the <see cref="System.Timers.ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> Expected the <see cref="System.Timers.Timer"/> object. </param>
        /// <param name="e"> Provide data for the <see cref="System.Timers.Timer.Elapsed"/> </param> event.
        private void SpeedTimerHandler(object sender, ElapsedEventArgs e)
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
        private void ElapseTimerHandler(object sender, ElapsedEventArgs e)
        {
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
