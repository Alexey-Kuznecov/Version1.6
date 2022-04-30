
namespace UnityCommander.Core.IO.Operations
{
    using System.IO;

    /// <summary>
    /// The file manager.
    /// </summary>
    public class FileManager
    {
        /// <summary>
        /// The move.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        public static void Move(string source, string target)
        {
            if (source == null || target == null) return;
            
            if (File.Exists(source))
            {
                var fileInfo = new FileInfo(source);
                File.Move(source, Path.Combine(target, fileInfo.Name));
            }
            else
            {
                var directoryInfo = new DirectoryInfo(source);
                Directory.Move(source, Path.Combine(target, directoryInfo.Name));
            }
        }

        /// <summary>
        /// The move each.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        public virtual void MoveEach(string source, string target)
        {
            foreach (var dir in Directory.GetDirectories(source))
            {
                string newDir = dir.Replace(source, target);
                Directory.Move(dir, newDir);
            }

            foreach (var file in new DirectoryInfo(source).GetFiles())
            {
                file.MoveTo(Path.Combine(target, file.Name));
            }
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public static void Delete(string source)
        {
            if (source == null) return;

            if (File.Exists(source))
            {
                File.Delete(source);
            }
            else
            {
                Directory.Delete(source, true);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dirName">
        /// The dir path.
        /// </param>
        public static void Create(string dirName)
        {
            if (dirName != null)
                Directory.CreateDirectory(dirName);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="extension">
        /// The extension.
        /// </param>
        public virtual void Create(string filePath, string extension)
        {
            File.Create(filePath);
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public virtual void GetProperties(string source)
        {
        }
    }
}
