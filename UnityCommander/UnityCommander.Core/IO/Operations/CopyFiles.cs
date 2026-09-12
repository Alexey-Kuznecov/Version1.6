#define Nlog

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.IO;
using UnityCommander.Native;
using UnityCommander.Native.Api;

namespace UnityCommander.Core.IO.Operations
{
    using Timer = System.Timers.Timer;

    [ManagerAttach(typeof(CopyManager))]
    public class CopyFiles : IDisposable
    {
        #region Fields
        private readonly IBackgroundWorkController _backgroundWork;

        private readonly Queue<long> Snapshots = new Queue<long>(30);
        private readonly Timer SpeedTimer = new Timer(1000D);
        private readonly Timer ElapsedTimer = new Timer(1000D);
        private CopyBehaviors copyBehaviors;
        private FileOperations fileOperation;
        private CopyInfo copyInfo;
        private long currentBytesTransferred;
        private long fileSize;
        private long totalFileSize;
        private long totalBytesLeft;
        private long lastBytes;
        private double lastByteDone;
        private double averageSpeed;
        private TimeSpan totalTimeLeft;
        public event EventHandler CopyReportEvent;
        public event EventHandler FileAlreadyExistsEvent;

        public event Action<CopyInfo> FileStarted;        // Начало копирования файла
        public event Action<CopyInfo> FileCompleted;      // Файл полностью скопирован
        public event Action<string> DirectoryCreated;     // Папка создана


        #endregion

        #region Test Properties

        public FileOperations Operations => fileOperation;
        public CopyInfo CopyInfoInstance => copyInfo;
        public Timer GetElapsedTimer => ElapsedTimer;
        public Timer GetSpeedTimer => SpeedTimer;

        public string TargetRoot { get; set; }
        public string SourceRoot { get; set; }

        #endregion

        public CopyFiles(OperationContext ctx)
        {
            _backgroundWork = ctx.BackgroundWork;
            fileOperation = new FileOperations();
            copyInfo = ctx.Info;
            SpeedTimer.Elapsed += SpeedTimerHandler;
            ElapsedTimer.Elapsed += ElapseTimerHandler;
        }

        public void Dispose()
        {
            SpeedTimer.Stop();
            ElapsedTimer.Stop();
        }

        public void ChangeCopyStatus(CopyBehaviors changeOn)
        {
            copyBehaviors = changeOn;
        }

        public void Copy(string source, string destinationFile)
        {
            var info = new FileInfo(source);

            totalFileSize = info.Length;
            copyInfo.TotalBytes += info.Length;

            SpeedTimer.Start();
            ElapsedTimer.Start();

            FileStarted?.Invoke(copyInfo);

            CopyFile(source, destinationFile);
        }
        
        public void CopyFile(
            string oldFile,
            string newFile)
        {
            var info = new FileInfo(oldFile);
            var infoF = new FileInfo(newFile);

            fileSize = info.Length;

            copyInfo.Name = info.Name;
            copyInfo.Length = info.Length;
            copyInfo.Source = info.FullName;
            copyInfo.DestinationPath = newFile;
            copyInfo.Root = TargetRoot;
            copyInfo.FileInfo = infoF;

            FileStarted?.Invoke(copyInfo);

            //if (File.Exists(newFile))
            //{
            //    copyBehaviors = CopyBehaviors.Pause;
            //    RaiseFileAlreadyExistsEvent(copyInfo);
            //}

            fileOperation.XCopy(
                oldFile,
                newFile,
                CopyProgressHandle);
        }

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

        internal void RaiseFileAlreadyExistsEvent(CopyInfo info)
        {
            if (this.FileAlreadyExistsEvent != null)
            {
                this.FileAlreadyExistsEvent.Invoke(this, new CopyReportEventArg(info));
            }
        }

        private void RaiseEvent(CopyInfo info)
        {
            if (CopyReportEvent != null)
            {
                CopyReportEvent.Invoke(null, new CopyReportEventArg(info));
            }
        }

        #region Private Methods

        private bool _throttleCopy = true;

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

                    _backgroundWork.Wait();

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
                            copyInfo.Percentage = (double)transferred / total * 100;
                            copyInfo.CurrentFileSize = total;
                            var calibration = transferred - lastBytes;
                            copyInfo.CurrentBytesTransferred = calibration;
                            CalculateTimeLeft(calibration);
                            Interlocked.Add(ref currentBytesTransferred, calibration);
                            lastBytes = transferred;
                            copyInfo.TotalByteDone = lastByteDone + transferred;
                            copyInfo.TotalPercentage = copyInfo.TotalByteDone / totalFileSize * 100;   
                            
                            if (transferred == total)
                            {
                                lastByteDone += transferred;
                                FileCompleted?.Invoke(copyInfo);
                            }
                            Interlocked.Exchange(ref copyInfo, copyInfo);
                            RaiseEvent(copyInfo);
                            return CopyProgressResult.PROGRESS_CONTINUE;
                    }

                default:
                    currentBytesTransferred = Interlocked.Add(ref currentBytesTransferred, transferred);
                    return CopyProgressResult.PROGRESS_CONTINUE;
            }
        }
        private void CalculateTimeLeft(long byteTransferred)
        {
            if (byteTransferred > 0)
            {
                var bps = (fileSize - lastBytes) / byteTransferred;
                var timeLeft = TimeSpan.FromSeconds(bps);
                copyInfo.TimeLeft = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
                totalBytesLeft += byteTransferred;
                copyInfo.TotalBytesTransferred = totalBytesLeft;
            }
        }

        #endregion

        #region Timer Handlers
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
