
using System.IO;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Common.Panels
{
    public sealed class FolderModelFactory
    {
        public FolderModel? Create(string path)
        {
            try
            {
                var info = new DirectoryInfo(path);

                return new FolderModel
                {
                    Name = info.Name,
                    Path = info.FullName,
                    CreationTime = info.CreationTime,
                    LastAccessTime = info.LastAccessTime,
                    TargetPanel = TargetPanel.Folders,
                    Key = info.FullName,
                    IconKey = "core.folder",
                    Kind = IconKind.Folder
                };
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
        }
    }
}
