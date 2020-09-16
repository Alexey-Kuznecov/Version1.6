
namespace UnityCommander.Test
{
    using System;
    using System.IO;
    using System.Security.Permissions;

    /// <summary>
    /// The directory watcher.
    /// </summary>
    public class DirectoryWatcher : IDisposable
    {
        /// <summary>
        /// The watcher.
        /// </summary>
        private static FileSystemWatcher watcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryWatcher"/> class.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public DirectoryWatcher(string path)
        {
            Watcher(path);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            watcher.Dispose();
        }

        /// <summary>
        /// The watcher.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void Watcher(string path)
        {
            // If a directory is not specified, exit program.
            if (string.IsNullOrEmpty(path))
            {
                // Display the proper way to call the program.
                Console.WriteLine("Usage: Watcher.exe (directory)");
                return;
            }

            // Create a new FileSystemWatcher and set its properties.
            watcher = new FileSystemWatcher();
            watcher.Path = path;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastAccess 
                                 | NotifyFilters.LastWrite 
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch text files.
            watcher.Filter = "*";

            // Add event handlers.
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// The on deleted.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="e"> The e. </param>
        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        /// <summary>
        /// The on created.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="e"> The e. </param>
        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        /// <summary>
        /// The on changed.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="e"> The e. </param>
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted
            Console.WriteLine($"File: {e.FullPath} {e.ChangeType}");
        }

        /// <summary>
        /// The on renamed.
        /// </summary>
        /// <param name="source"> The source. </param>
        /// <param name="e"> The e. </param>
        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine($"File: {e.OldFullPath} renamed to {e.FullPath}");
        }
    }
}
