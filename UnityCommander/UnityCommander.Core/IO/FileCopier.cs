
namespace UnityCommander.Core.IO
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

    /// <summary>
    /// The file copier.
    /// </summary>
    public class FileCopier
    {
        #region Declaration Fields

        /// <summary>
        /// The Snapshots.
        /// </summary>
        private static readonly Queue<long> Snapshots = new Queue<long>(30);

        /// <summary>
        /// The Timer.
        /// </summary>
        private static readonly System.Timers.Timer Timer = new System.Timers.Timer(1000D);

        /// <summary>
        /// The current bytes transferred.
        /// </summary>
        private static long currentBytesTransferred;

        /// <summary>
        /// The total bytes transferred.
        /// </summary>
        private static long totalBytesTransferred;

        /// <summary>
        /// The file size.
        /// </summary>
        private static long fileSize;

        /// <summary>
        /// The average speed.
        /// </summary>
        private static long remain;

        /// <summary>
        /// The copyControl.
        /// </summary>
        private static CopyControl copyControl;

        /// <summary>
        /// The operations.
        /// </summary>
        private static FileOperations operations;

        /// <summary>
        /// The data model for storing information about the progress of file copying.
        /// </summary>
        private static CopyInfo copyInfoInstance;

        /// <summary>
        /// The copy progress report.
        /// </summary>
        public static event EventHandler<CopyInfo> CopyProgressReport;

        /// <summary>
        /// The copy file resualt.
        /// </summary>
        public static event EventHandler<CopyInfo> CopyFileResult;

        #endregion

        /// <summary>
        /// The flags external control of the copy behavior.
        /// </summary>
        public enum CopyControl : byte
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
        /// Indicates the status of copying.
        /// </summary>
        public enum CopyStatus : byte
        {
            /// <summary>
            /// The finished.
            /// </summary>
            Finished = 0,

            /// <summary>
            /// The running.
            /// </summary>
            Running = 1,

            /// <summary>
            /// The cancelled.
            /// </summary>
            Cancelled = 2,

            /// <summary>
            /// The waiting.
            /// </summary>
            Waiting = 3,
        }

        /// <summary>
        /// This method can stop, resume, or cancel the copy altogether.
        /// </summary>
        /// <param name="changeOn"> Select copyControl to change copy behavior. </param>
        public static void ChangeCopyStatus(CopyControl changeOn)
        {
            copyControl = changeOn;
        }

        /// <summary>
        /// Copies a each file of the directory to new location.
        /// </summary>
        /// <param name="src">
        /// The part to the source files.
        /// </param>
        /// <param name="dest">
        /// The part to the new location.
        /// </param>
        public static void StartCopyDeep(string src, string dest)
        {
            Task.Factory.StartNew(() =>
            {
                operations = new FileOperations();
                Timer.Elapsed += Timer_Elapsed;
                
                foreach (var oldDir in Directory.GetDirectories(src, "*", SearchOption.AllDirectories))
                {
                    var newDir = oldDir.Replace(src, dest);
                    Directory.CreateDirectory(newDir);
                    CopyFiles(oldDir, newDir);
                    CopyFileResult?.Invoke(null, copyInfoInstance);
                }

                if (Directory.GetFiles(src).Length != 0)
                {
                    CopyFiles(src, dest);
                }
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
        private static void CopyFiles(string oldDir, string newDir)
        {
            foreach (var oldFile in Directory.GetFiles(oldDir))
            {
                FileInfo info = new FileInfo(oldFile);
                string newFile = Path.Combine(newDir, new DirectoryInfo(oldFile).Name);

                fileSize = info.Length;
                copyInfoInstance = new CopyInfo
                {
                    Name = info.Name,
                    Length = info.Length,
                    Source = info.FullName,
                    Destination = newFile
                };

                if (!File.Exists(newFile))
                {
                    Timer.Start();
                    copyInfoInstance.Skipped = operations.XCopy(oldFile, newFile, CopyProgressHandle);
                    Timer.Stop();
                }
            }
        }

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
                    switch (copyControl)
                    {
                        case CopyControl.Pause:
                            var timeout = TimeSpan.FromMinutes(10D);
                            var mre = new ManualResetEvent(false);
                            while (copyControl != CopyControl.Resume)
                            {
                                mre.Set();
                                mre.WaitOne(timeout);
                            }

                            return CopyProgressResult.PROGRESS_CONTINUE;
                        case CopyControl.Cancel:
                            return CopyProgressResult.PROGRESS_CANCEL;
                        default:
                            // Initial the CopyInfo object. 
                            copyInfoInstance.Percentage = (double)transferred / total * 100;
                            copyInfoInstance.ByteDone = transferred;
                            copyInfoInstance.TotalByte = total;

                            // Correcting the value for correct timer operation.
                            var calibration = transferred - remain;

                            // Calculate the time left and the average speed.
                            currentBytesTransferred = Interlocked.Add(ref currentBytesTransferred, calibration);
                            totalBytesTransferred = Interlocked.Add(ref totalBytesTransferred, calibration);

                            // Correcting the value for correct timer operation.
                            remain = transferred;

                            // Report current copy progress.
                            Interlocked.Exchange<CopyInfo>(ref copyInfoInstance, copyInfoInstance);
                            CopyProgressReport?.Invoke(null, copyInfoInstance);

                            return CopyProgressResult.PROGRESS_CONTINUE;
                    }  

            default:
                    currentBytesTransferred = Interlocked.Add(ref currentBytesTransferred, transferred);
                    return CopyProgressResult.PROGRESS_CONTINUE;
            }
        }

        /// <summary>
        /// Calculates the time left and the average speed when occurring the <see cref="ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> Expected the <see cref="System.Timers.Timer"/> object. </param>
        /// <param name="e"> Provide data for the <see cref="System.Timers.Timer.Elapsed"/> </param> event.
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Remember only the last 30 Snapshots; discard older Snapshots
            if (Snapshots.Count == 30)
            {
                Snapshots.Dequeue();
            }

            Snapshots.Enqueue(Interlocked.Exchange(ref currentBytesTransferred, 0L));
            var averageSpeed = Snapshots.Average();
            var bytesLeft = fileSize - totalBytesTransferred;
            copyInfoInstance.AverageSpeed = averageSpeed / (1024 * 1024);
            if (averageSpeed > 0)
            { 
                var timeLeft = TimeSpan.FromSeconds(bytesLeft / averageSpeed);
                copyInfoInstance.TimeLeftRounded = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
            }
            else
            {
                copyInfoInstance.TimeLeftRounded = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// The copy info.
        /// </summary>
        public class CopyInfo
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the source directory.
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// Gets or sets the destination directory.
            /// </summary>
            public string Destination { get; set; }

            /// <summary>
            /// Gets or sets the length.
            /// </summary>
            public long Length { get; set; }

            /// <summary>
            /// Gets or sets the average speed.
            /// </summary>
            public double AverageSpeed { get; set; }

            /// <summary>
            /// Gets or sets the time left rounded.
            /// </summary>
            public TimeSpan TimeLeftRounded { get; set; }

            /// <summary>
            /// Gets or sets the percentage.
            /// </summary>
            public double Percentage { get; set; }

            /// <summary>
            /// Gets or sets the byte done.
            /// </summary>
            public double ByteDone { get; set; }

            /// <summary>
            /// Gets or sets the total byte.
            /// </summary>
            public double TotalByte { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether there are copy errors.
            /// </summary>
            public bool Skipped { get; set; }

            /// <summary>
            /// Gets value the copying state.
            /// </summary>
            public CopyStatus CopyStatusOption { get; private set; }
        }
    }
}
