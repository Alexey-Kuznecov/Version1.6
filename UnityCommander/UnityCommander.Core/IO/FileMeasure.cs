
namespace UnityCommander.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Timers;
    using System.Windows.Documents;

    using Timer = System.Timers.Timer;

    /// <summary>
    /// The file measure.
    /// </summary>
    public class FileMeasure
    {
        /// <summary>
        /// The Snapshots.
        /// </summary>
        private static readonly Queue<long> Snapshots = new Queue<long>(30);

        /// <summary>
        /// The Timer.
        /// </summary>
        private static readonly Timer Timer = new System.Timers.Timer(1000D);

        /// <summary>
        /// The total size.
        /// </summary>
        private double totalBytes;

        /// <summary>
        /// The remain bytes.
        /// </summary>
        private double remainBytes;

        /// <summary>
        /// The file size.
        /// </summary>
        private double fileSize;

        /// <summary>
        /// The total bytes transferred.
        /// </summary>
        private double totalBytesTransferred;

        /// <summary>
        /// The copy info instance.
        /// </summary>
        private double copyInfoInstance;

        /// <summary>
        /// The current bytes transferred.
        /// </summary>
        private double currentBytesTransferred;

        /// <summary>
        /// Gets or sets the time left rounded.
        /// </summary>
        public TimeSpan TimeLeftRounded { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FileMeasure"/> class.
        /// </summary>
        public FileMeasure()
        {
            Timer.Elapsed += this.Timer_Elapsed;
        }

        /// <summary>
        /// This method is created.
        /// </summary>
        /// <param name="paths">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double MeasureTotalFileSize(List<string> paths)
        {
            this.totalBytes = 0;

            foreach (var path in paths)
            {
                FileInfo info = new FileInfo(path);
                this.totalBytes += info.Length;
            }

            return this.totalBytes;
        }

        /// <summary>
        /// Calculates the time left and the average speed when occurring the <see cref="ElapsedEventHandler"/> event.
        /// </summary>
        /// <param name="sender"> Expected the <see cref="System.Timers.Timer"/> object. </param>
        /// <param name="e"> Provide data for the <see cref="System.Timers.Timer.Elapsed"/> </param> event.
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Remember only the last 30 Snapshots; discard older Snapshots
            if (Snapshots.Count == 30)
            {
                Snapshots.Dequeue();
            }

            //Snapshots.Enqueue(Interlocked.Exchange(ref this.currentBytesTransferred, 0L));
            //var averageSpeed = Snapshots.Average();
            //var bytesLeft = this.fileSize - this.totalBytesTransferred;
            //this.copyInfoInstance.AverageSpeed = averageSpeed / (1024 * 1024);
            //if (averageSpeed > 0)
            //{
            //    var timeLeft = TimeSpan.FromSeconds(bytesLeft / averageSpeed);
            //    this.TimeLeftRounded = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
            //}
            //else
            //{
            //    this.TimeLeftRounded = TimeSpan.Zero;
            //}
        }
    }
}
