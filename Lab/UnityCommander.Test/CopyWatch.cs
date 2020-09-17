using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace UnityCommander.Test
{
    public class CopyWatch
    {
         /// <summary>
         /// The Logger.
         /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private 
        public ProcessCounter Counter { get; }
        public static long TotalBytes { get; set; } = 3160233984; // 2,94 gb
        public double BytesCounter { get; set; }
        public static long Speed { get; set; }
        public static decimal RemainTime { get; set; }

        public CopyWatch(Process process)
        {
            LogManager.Configuration = new XmlLoggingConfiguration(@"C:\Users\Misha\Desktop\Version1.8\Lab\UnityCommander.WinDepends\NLog.config");
            Counter = new ProcessCounter(process);
        }

        public void WatchProcess()
        {
            
            Task task = Task.Factory.StartNew(CalcBytesInterval);
            Task task2 = Task.Factory.StartNew(CalcTime);
            Speed = (long)Counter.IOReadBytes.NextValue();

            if (Speed != 0)
            {
                RemainTime = Math.Truncate((decimal)((TotalBytes - (long)BytesCounter) / Speed));

                Logger.Info("\nTotalBytes: {0}\nBytesCounter {1}\nSpeed {2}\n RemainTime {3}", TotalBytes, BytesCounter, Speed, RemainTime);
            }

            Show();

            task.Wait();
        }

        public void Show()
        {
            if (Speed != 0)
            {
                Console.WriteLine("Received Auto: {0}", ConverterBytes.AutoConvertFormatBytes((decimal)BytesCounter));
                Console.WriteLine("Speed: {0}/s", ConverterBytes.AutoConvertFormatBytes(Speed));
                Console.WriteLine("Remain Time: {0} second", RemainTime);
                Console.WriteLine(new string('-', 20));
            }
        }

        public void CalcBytesInterval()
        {
            int seconds = 16;
            int counter = 0;

            while (counter++ <= seconds)
            {
                BytesCounter += (double)Counter.IOWriteBytes.NextValue() / 29.5;
            }
        }

        public void CalcTime()
        {
            int seconds = 16;
            int counter = 0;

            while (counter++ <= seconds)
            {
                BytesCounter += (double)Counter.IOWriteBytes.NextValue() / 29.5;
            }
        }
    }
}
