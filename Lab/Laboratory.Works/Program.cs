
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
            Console.ReadKey();
        }

        /// <summary>
        /// The get foreground window.
        /// </summary>
        /// <returns> The <see cref="IntPtr"/>. </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// The get window thread process id.
        /// </summary>
        /// <param name="hwnd"> The hwnd. </param>
        /// <param name="pid"> The pid. </param>
        /// <returns> The <see cref="uint"/>. </returns>
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, ref int pid);

        /// <summary>
        /// The method.
        /// </summary>
        public static void Method()
        {
            int pid = 1464;

            GetWindowThreadProcessId(GetForegroundWindow(), ref pid);
            Process process = Process.GetProcessById(pid);

            var desc = process.MainModule?.FileVersionInfo.FileDescription;
            var title = process.MainWindowTitle;
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(process.MainModule.FileName);
        }
    }
}
