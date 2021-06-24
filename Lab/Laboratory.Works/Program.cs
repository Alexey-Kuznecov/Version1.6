
namespace Laboratory.Works
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

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
           CopyWatchTest.Start();
        }

        ///// <summary>
        ///// The method.
        ///// </summary>
        //public static void Method()
        //{
        //    int pid = 1464;
        //    Process currentProcess = Process.GetCurrentProcess();
        //    NativeFunctions.GetWindowThreadProcessId(NativeFunctions.GetForegroundWindow(), ref pid);
        //    Process process = Process.GetProcessById(pid);

        //    var desc = currentProcess.MainModule?.FileVersionInfo.FileDescription;
        //    var title = currentProcess.MainWindowTitle;
            
        //    if (currentProcess.MainModule != null)
        //    {
        //        var fileVersionInfo = FileVersionInfo.GetVersionInfo(currentProcess.MainModule.FileName);
        //    }
        //}
    }
}
