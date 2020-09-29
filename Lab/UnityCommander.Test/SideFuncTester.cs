using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace UnityCommander.Test
{
    public class SideFuncTester
    {
        static string sourcePath = @"E:\Temp\1\JetBrains.Rider-2019.2.2.exe";
        static string destinationPath = @"E:\Temp\2\JetBrains.Rider-2019.2.2.exe";
        static long currentBytesTransferred;
        static long totalBytesTransferred;
        static Queue<long> snapshots = new Queue<long>(30);
        static System.Timers.Timer timer = new System.Timers.Timer(1000D);
        static long fileSize;
        /// <summary>
        /// Here's a working example of how you would asynchronously copy a file D:\dummy.bin to D:\dummy.bin.copy,
        /// with a timer taking snapshots of the transfer rate every second.
        /// From that data, I simply take the average transfer rate from up to 30 snapshots(newest first). 
        /// From that I can calculate a rough estimate of how long it will take to transfer the rest of the file.
        /// This example is provided as-is and does not support copying multiple files in 1 operation.But it should give you some ideas.
        /// </summary>
        /// <remarks> Source: https://stackoverrun.com/ru/q/5981401 </remarks>
        /// <param name="args"></param>
        public static void CopyAsync(string src, string dest)
        {
            FileInfo sourceFile = new FileInfo(src);
            fileSize = sourceFile.Length;
            timer.Elapsed += Timer_Elapsed;
            using (var inputStream = sourceFile.OpenRead())
            using (var outputStream = File.OpenWrite(dest))
            {
                timer.Start();
                var buffer = new byte[4096];
                var numBytes = default(int);
                var numBytesMax = buffer.Length;
                var timeout = TimeSpan.FromMinutes(10D);
                do
                {
                    var mre = new ManualResetEvent(false);
                    inputStream.BeginRead(buffer, 0, numBytesMax, asyncReadResult =>
                    {
                        numBytes = inputStream.EndRead(asyncReadResult);
                        outputStream.BeginWrite(buffer, 0, numBytes, asyncWriteResult =>
                        {
                            outputStream.EndWrite(asyncWriteResult);
                            currentBytesTransferred = Interlocked.Add(ref currentBytesTransferred, numBytes);
                            totalBytesTransferred = Interlocked.Add(ref totalBytesTransferred, numBytes);
                            mre.Set();
                        }, null);
                    }, null);
                    
                } while (numBytes != 0);
                timer.Stop();
            }
        }

        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Remember only the last 30 snapshots; discard older snapshots
            if (snapshots.Count == 30)
            {
                snapshots.Dequeue();
            }

            snapshots.Enqueue(Interlocked.Exchange(ref currentBytesTransferred, 0L));
            var averageSpeed = snapshots.Average();
            var bytesLeft = fileSize - totalBytesTransferred;
            Console.WriteLine("Average speed: {0:#} MBytes / second", averageSpeed / (1024 * 1024));
            if (averageSpeed > 0)
            {
                var timeLeft = TimeSpan.FromSeconds(bytesLeft / averageSpeed);
                var timeLeftRounded = TimeSpan.FromSeconds(Math.Round(timeLeft.TotalSeconds));
   
                Console.WriteLine("Time left: {0}", timeLeftRounded);
            }
            else
            {
                Console.WriteLine("Time left: Infinite");
            }
        }

        static void Test()
        {
            bool quit = false;
            System.DateTime dt = DateTime.Now;
            do
            {
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(false).Key == ConsoleKey.Escape)
                        quit = true;
                }
                System.Threading.Thread.Sleep(0);
                if (DateTime.Now.Subtract(dt).TotalSeconds > .1)
                {
                    dt = DateTime.Now;
                    WriteOut(dt.ToString(" ss.ff"), false);
                }
            } while (!quit);
        }

        private static int outRow;
        private static int outHeight;
        private static int outCol;

        static void WriteOut(string msg, bool appendNewLine)
        {
            int inCol, inRow;
            inCol = Console.CursorLeft;
            inRow = Console.CursorTop;

            int outLines = getMsgRowCount(outCol, msg) + (appendNewLine ? 1 : 0);
            int outBottom = outRow + outLines;
            if (outBottom > outHeight)
                outBottom = outHeight;
            if (inRow <= outBottom)
            {
                int scrollCount = outBottom - inRow + 1;
                Console.MoveBufferArea(0, inRow, Console.BufferWidth, 1, 0, inRow + scrollCount);
                inRow += scrollCount;
            }
            if (outRow + outLines > outHeight)
            {
                int scrollCount = outRow + outLines - outHeight;
                Console.MoveBufferArea(0, scrollCount, Console.BufferWidth, outHeight - scrollCount, 0, 0);
                outRow -= scrollCount;
                Console.SetCursorPosition(outCol, outRow);
            }
            Console.SetCursorPosition(outCol, outRow);
            if (appendNewLine)
                Console.WriteLine(msg);
            else
                Console.Write(msg);
            outCol = Console.CursorLeft;
            outRow = Console.CursorTop;
            Console.SetCursorPosition(inCol, inRow);
        }

        private static int getMsgRowCount(object outCol, string msg)
        {
            throw new NotImplementedException();
        }

        static int getMsgRowCount(int startCol, string msg)
        {
            string[] lines = msg.Split('\n');
            int result = 0;
            foreach (string line in lines)
            {
                result += (startCol + line.Length) / Console.BufferWidth;
                startCol = 0;
            }
            return result + lines.Length - 1;
        }
    }
}
