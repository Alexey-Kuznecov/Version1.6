
#define Nlog

namespace UnityCommander.Test
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using NLog;
    using NLog.Config;

    /// <summary>
    /// The copy watch.
    /// </summary>
    public class CopyWatch
    {
#if (Nlog)
        /// <summary>
        /// The Logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyWatch"/> class.
        /// </summary>
        /// <param name="process">
        /// The process.
        /// </param>
        public CopyWatch(Process process)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("G:\\Works\\UnityCommander\\Version2.6\\NLog.config");
            this.Counter = new ProcessCounter(process);
        }

         /// <summary>
         /// Gets or sets the total bytes.
         /// </summary>
         public static long TotalBytes { get; set; } = 484261288; // 2,94 gb

         /// <summary>
         /// Gets or sets the speed.
         /// </summary>
         public static long Speed { get; set; }

         /// <summary>
         /// Gets or sets the remain time.
         /// </summary>
         public static TimeSpan RemainTime { get; set; }

         /// <summary>
         /// Gets the counter.
         /// </summary>
         public ProcessCounter Counter { get; }

         /// <summary>
         /// Gets or sets the bytes counter.
         /// </summary>
         public double BytesCounter { get; set; }

         /// <summary>
         /// The watch process.
         /// </summary>
         public async void WatchProcess()
         {
             // TODO: Task doesn't work when debugging something breaks.
             bool taskCalcBytes = await Task<bool>.Factory.StartNew(this.CalcBytesInterval).ConfigureAwait(true);
             bool taskCalcTime = await Task<bool>.Factory.StartNew(this.CalcTime).ConfigureAwait(true);

             if (!taskCalcBytes && !taskCalcTime) return;
#if (Nlog)
             if (Speed != 0)
             {
                 Logger.Info("\nTotalBytes: {0}\nSpeed {1}", TotalBytes, Speed);
                 Logger.Info("BytesCounter: {0}", this.BytesCounter);
                 Logger.Info("RemainTime: {0}", RemainTime);
                 Logger.Info(new string('-', 30));
             }
#endif
             this.Show();
         }

         /// <summary>
         /// The show.
         /// </summary>
         public void Show()
         {
             if (Speed != 0)
             {
                 Console.WriteLine("Received Auto: {0}", ConverterBytes.AutoConvertFormatBytes((decimal)this.BytesCounter));
                 Console.WriteLine("Speed: {0}/s", ConverterBytes.AutoConvertFormatBytes(Speed));
                 Console.WriteLine("Remain Time: {0} second", RemainTime.Seconds);
                 Console.WriteLine(new string('-', 20));
             }
         }

         /// <summary>
         /// The calc bytes interval.
         /// </summary>
         /// <returns>
         /// The <see cref="bool"/>.
         /// </returns>
         public bool CalcBytesInterval()
         {
             int seconds = 16;
             int counter = 0;
         
             while (counter++ <= seconds)
             {
                 // TODO: This expression will be works only without using the threads.
                 this.BytesCounter += this.Counter.IOWriteBytes.NextValue() / 4;
             }

#if (Nlog)
            if (Speed != 0)
            {
                Logger.Info("BytesCounter: {0}", this.BytesCounter);
            }
#endif
             return true;
        }

         /// <summary>
         /// The calc time.
         /// </summary>
         /// <returns>
         /// The <see cref="bool"/>.
         /// </returns>
         public bool CalcTime()
         {
            if (Speed != 0)
            {
                // Thread.Sleep(200);
                Speed = (long)this.Counter.IOReadBytes.NextValue();
                RemainTime = TimeSpan.FromMilliseconds((TotalBytes - this.BytesCounter) / Speed);
#if (Nlog)
                Logger.Info("RemainTime: {0}", RemainTime);
#endif
            }

            return true;
         }
    }
}
