
namespace UnityCommander.Test
{
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
            CopyWatchTest.Start();

            // Gets open file handle used by process.
            // ProcessMonitorTest.Start();
        }
    }
}