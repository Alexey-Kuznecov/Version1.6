
namespace UnityCommander.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityCommander.Native;
    using UnityCommander.Test.TestStart;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            FileCopierTest.Start();
            // Gets open file handle used by process.
            // ProcessMonitorTest.Start();
        }
    }
}