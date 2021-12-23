
using UnityCommander.Test.IO;

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
            var source = "D:\\Works\\WPF\\CopyTest\\Source";
            var target = "D:\\Works\\WPF\\CopyTest\\Target";
            var file = @"D:\Works\WPF\CopyTest\Target\Adobe Acrobat Reader.rar";

            FileManager manager = new FileManager();
            manager.Create(file, ".txt");
        }
    }
}