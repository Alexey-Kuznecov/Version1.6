
namespace UnityCommander.Test
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using AkuzIcons.Shell;
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
            IconProvider iconProvider = new IconProvider();
            //DrawingBrush brush = iconProvider.GetIcon();
            var resource = new ResourceDictionary
            {
                Source = new Uri("/AkuzIcons;component/IconPack.xaml")
            };

            // Gets open file handle used by process.
            // ProcessMonitorTest.Start();
        }
    }
}