
namespace Laboratory.Works
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using UnityCommander.WinDepends;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args"> The args. </param>
        public static void Main(string[] args)
        {
            List<FileSystemInfo> infos = new List<FileSystemInfo>();

            var process = Process.GetProcessesByName("TOTALCMD64").Select(p => p.Id).ToList();

            while (true)
            {
                // Thread.Sleep(500);
                infos = new List<FileSystemInfo>();

                using (var openFiles = DetectOpenFiles.GetOpenFilesEnumerator(process[0]))
                {
                    while (openFiles.MoveNext())
                    {
                        infos.Add(openFiles.Current);
                    }
                } // Process.GetCurrentProcess().Id))

                infos.Sort(delegate (FileSystemInfo x, FileSystemInfo y)
                    {
                        string strX = x.FullName.Substring(0, 5);
                        string strY = y.FullName.Substring(0, 5);
                        return string.Compare(strX, strY, StringComparison.Ordinal);
                    });

                foreach (var fileSystem in infos)
                {
                    Console.WriteLine(fileSystem.FullName);
                }

                Console.WriteLine(new string('—', 50));
            }

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
