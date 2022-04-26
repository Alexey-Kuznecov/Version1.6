using System.IO;
using UnityCommander.Common;
using UnityCommander.Integration.Commands;

namespace UnityCommander.Core.IO.Operations
{
    public class FileManager : IOCommands
    {
        [GlobalCommand("FileSelMove")]
        public override void Move(string source, string target)
        {
            return;
    
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

        public void MoveEach(string source, string target)
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

        [GlobalCommand("FileSelDel")]
        public void Delete(string source)
        {
            return;

            if (File.Exists(source))
            {
                File.Delete(source);
            }
            else
            {
                Directory.Delete(source, true);
            }
        }

        public void Create(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
        }

        public void Create(string filePath, string extension)
        {
            File.Create(filePath);
        }

        public void GetProperties(string source)
        {
        }
    }
}
