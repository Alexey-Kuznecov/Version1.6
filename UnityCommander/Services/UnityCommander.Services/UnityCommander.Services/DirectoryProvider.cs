
namespace UnityCommander.Services
{
    using System.Collections.ObjectModel;
    using System.IO;
    using UnityCommander.Common.Enums;
    using UnityCommander.Common.Models;
    using UnityCommander.Services.Interfaces;
    
    /// <summary>
    /// The files provider.
    /// </summary>
    public class DirectoryProvider : IDirectoryProvider
    {
        /// <summary>
        /// Gets files list of the specific location.
        /// </summary>
        /// <param name="path"> The path to the directory where the files are located. </param>
        /// <returns> The files collection. </returns>
        public ObservableCollection<FileModel> GetFiles(string path)
        {
            ObservableCollection<FileModel> models = new ObservableCollection<FileModel>();
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (var file in dir.GetFiles())
            {
                if ((file.Attributes & FileAttributes.Hidden) == 0)
                {
                    models.Add(new FileModel
                    {
                        Name = Path.GetFileNameWithoutExtension(file.Name),
                        Path = file.FullName,
                        Extension = file.Extension,
                        CreationTime = file.CreationTime,
                        LastAccessTime = file.LastAccessTime,
                        Type = DirectoryItemType.Files
                    });
                }
            }

            return models;
        }

        /// <summary>
        /// Gets directories list of the specific location.
        /// </summary>
        /// <param name="path"> The path to the directory where the directories are located. </param>
        /// <returns> The directories collection. </returns>
        public ObservableCollection<FolderModel> GetDirectories(string path)
        {
            ObservableCollection<FolderModel> models = new ObservableCollection<FolderModel>();
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (var item in dir.GetDirectories())
            {
                if ((item.Attributes & FileAttributes.Hidden) == 0)
                {
                    models.Add(new FolderModel
                    {
                        Name = item.Name,
                        Path = item.FullName,
                        CreationTime = item.CreationTime,
                        LastAccessTime = item.LastAccessTime,
                        Type = DirectoryItemType.Folder
                    }); ;
                }
            }

            return models;
        }
    }
}