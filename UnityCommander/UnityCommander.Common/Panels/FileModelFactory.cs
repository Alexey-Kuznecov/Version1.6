
using System.IO;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Common.Panels
{
    public class FileModelFactory
    {
        public FileModel Create(string path)
        {
            var info = new FileInfo(path);

            return new FileModel
            {
                Name = Path.GetFileNameWithoutExtension(info.Name),
                Path = info.FullName,
                Extension = info.Extension,
                CreationTime = info.CreationTime,
                LastAccessTime = info.LastAccessTime,
                TargetPanel = TargetPanel.Files,
                Key = info.FullName,
                Size = info.Length,
                IconKey = "core.file",
                Kind = IconKind.File
            };
        }
    }
}
