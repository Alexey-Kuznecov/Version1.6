
namespace UnityCommander.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using UnityCommander.Native;
    using UnityCommander.Native.Api;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The progress.
        /// </summary>
        private static ProgressBar progress;

        /// <summary>
        /// The progress percentage.
        /// </summary>
        private static byte progressPerc;


        private static double percent = 100;

        private static double mg = 0;

        private static double derive = 0;

        private static long pos = 0;

        private static int counter = 0;
        private static ProgressBar progressBar;

        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            FileOperations operations = new FileOperations();

            foreach (var item in Directory.GetFiles("E:\\temp\\", "*", SearchOption.TopDirectoryOnly))
            {
                counter = 0;
                derive = 0;
                pos = 0;
                FileInfo info = new FileInfo(item);
                Console.WriteLine("Copy File: {0} Size: {1}", info.Name, ConverterBytes.AutoConvertFormatBytes(info.Length));
                operations.XCopy(info.FullName, @"F:\target\\" + info.Name, CopyProgressHandle);
                //using (progressBar = new ProgressBar())
                //{
                //    operations.XCopy(info.FullName, @"F:\target\\" + info.Name, CopyProgressHandle);
                //}

                Console.WriteLine("File ready..");
                Console.WriteLine(new string('-', 50));
            }

            // CopyFileByte(@"H:\Works\UnitTests\Source\JetBrains.Rider-2019.2.2.exe", @"E:\Temp\2\JetBrains.Rider-2019.2.2.exe");
            Console.ReadKey();
        }

        /// <summary>
        /// The copy progress handler.
        /// </summary>
        /// <param name="total"> The total. </param>
        /// <param name="transferred"> The transferred. </param>
        /// <param name="streamSize"> The stream size. </param>
        /// <param name="streamByteTrans"> The stream byte trans. </param>
        /// <param name="dwStreamNumber"> The dw stream number. </param>
        /// <param name="reason"> The reason. </param>
        /// <param name="hSourceFile"> The h source file. </param>
        /// <param name="hDestinationFile"> The h destination file. </param>
        /// <param name="lpData"> The lp data. </param>
        /// <returns> The <see cref="CopyProgressResult"/>. </returns>
        public static CopyProgressResult CopyProgressHandle(long total, long transferred, long streamSize, long streamByteTrans, uint dwStreamNumber, CopyProgressCallbackReason reason, IntPtr hSourceFile, IntPtr hDestinationFile, IntPtr lpData)
        {
            if (reason == CopyProgressCallbackReason.CALLBACK_STREAM_SWITCH)
            {
                // int d = (int)(total % 100);
                mg = ((double)total / 1024 / 1024);
                percent = mg / 100;
            }

            if (++pos >= derive)
            {
                Console.Write(counter++ + " / ");
                //progressBar.Report((double)counter++ / 100);
                derive += percent;
            }

            return CopyProgressResult.PROGRESS_CONTINUE;
        }

        /// <summary>
        /// Copies the file by byte using stream of the files.
        /// </summary>
        /// <param name="source"> The source of the files. </param>
        /// <param name="destination"> The destination of the files. </param>
        public static void CopyFileByte(string source, string destination)
        {
            int count = 0, b = 0;
            int fileCopyIndicator = 0;

            using (var inFileStream = new FileStream(source, FileMode.Open))
            using (var outFileStream = new FileStream(destination, FileMode.Create))
            {
                long fileSizeByte = inFileStream.Length;

                // Gets a percentage of the file size and subtracts another 100. 
                // This is how I fixed a boolean error that affects the progress bar result.
                // Before the fix, the progress bar worked at 99%.
                var byteInPercent = fileSizeByte / 100;

                while ((b = inFileStream.ReadByte()) >= 0)
                {
                    outFileStream.WriteByte((byte)b);
                    count++;

                    // This part of the code serves to fix the moment at 
                    // which you can see the progress of copying the file.
                    if (count > byteInPercent)
                    {
                        //progress.Report((double)fileCopyIndicator++ / 98);
                        count = 0;
                    }
                }
            }
        }

        /// <summary>
        /// The automation copy.
        /// </summary>
        private static void AutomationCopy()
        {
            string path = "E:\\Temp\\JetBrains.Rider-2019.2.2.exe";

            Console.WriteLine("To start press Y: ");
            Console.WriteLine("To exit press Q: ");
            string flag = Console.ReadLine();

            do
            {
                if (File.Exists(path))
                {
                    Console.WriteLine("File already exist, to delete? ");

                    if (Console.ReadLine() == "y")
                    {
                        File.Delete(path);
                        Console.Clear();
                    }
                    else
                    {
                        flag = "q";
                    }
                }

                if (flag == "y")
                {
                    Console.Clear();
                    Console.Write("Copying files... ");
                    using (progress = new ProgressBar())
                    {
                        CopyFileByte(
                            "H:\\Works\\UnitTests\\Source\\JetBrains.Rider-2019.2.2.exe",
                            path);
                    }

                    Console.WriteLine("Done.");
                }

                Console.Clear();
                Console.WriteLine("To start press Y");
                Console.WriteLine("To exit press Q");
                flag = Console.ReadLine() == "q" ? "q" : "y";
            }
            while (flag == "q");
        }


        /// <summary>
        /// The vmc controller test.
        /// </summary>
        private static void VmcControllerTest()
        {
            List<FileSystemInfo> infos = new List<FileSystemInfo>();
            var process = Process.GetProcessesByName("TOTALCMD64").Select(p => p.Id).ToList();
            infos = new List<FileSystemInfo>();

            using (var openFiles = ProcessUtility.GetOpenFilesEnumerator(process[0]))
            {
                while (openFiles.MoveNext())
                {
                    infos.Add(openFiles.Current);
                }
            }

            infos.Sort(SortPaths);

            foreach (var fileSystem in infos)
            {
                Console.WriteLine(fileSystem.FullName);
            }
        }

        /// <summary>
        /// The sort paths.
        /// </summary>
        /// <param name="x"> The x. </param>
        /// <param name="y"> The y. </param>
        /// <returns> The <see cref="int"/>. </returns>
        private static int SortPaths(FileSystemInfo x, FileSystemInfo y)
        {
            string strX = x.FullName;
            string strY = y.FullName;
            int min = Math.Min(strX.Length, strY.Length);

            if (min > 5)
            {
                return string.Compare(strX.Substring(0, 5), strY.Substring(0, 5), StringComparison.Ordinal);
            }

            return string.Compare(strX.Substring(0, 2), strY.Substring(0, 2), StringComparison.Ordinal);
        }
    }
}