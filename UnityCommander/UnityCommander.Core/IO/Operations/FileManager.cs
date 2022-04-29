
namespace UnityCommander.Core.IO.Operations
{
    using System.IO;

    using UnityCommander.Common.Commands;

    /// <summary>
    /// The file manager.
    /// </summary>
    public class FileManager : BaseCommand
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
        [GlobalCommand(CommandNames.FileMove, CommandKeys.CtrlG)]
        public virtual void Move(string source, string target)
        {
            if (File.Exists(source))
            {
                FileInfo fileInfo = new FileInfo(source);
                File.Move(source, Path.Combine(target, fileInfo.Name));
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(source);
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
        [GlobalCommand(CommandNames.FileDel, CommandKeys.CtrlH)]
        public virtual void Delete(string source)
        {
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
        /// <param name="dirPath">
        /// The dir path.
        /// </param>
        [GlobalCommand(CommandNames.FileCreate, CommandKeys.CtrlI)]
        public virtual void Create(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
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
