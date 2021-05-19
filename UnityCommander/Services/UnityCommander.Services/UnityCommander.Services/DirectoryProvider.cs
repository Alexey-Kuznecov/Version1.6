
namespace UnityCommander.Services
{
    using System.Collections.ObjectModel;
    using System.IO;
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
                        Extension = file.Extension
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
        public ObservableCollection<DirectoryModel> GetDirectories(string path)
        {
            ObservableCollection<DirectoryModel> models = new ObservableCollection<DirectoryModel>();
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (var item in dir.GetDirectories())
            {
                if ((item.Attributes & FileAttributes.Hidden) == 0)
                {
                    models.Add(new DirectoryModel
                    {
                        Name = item.Name,
                        Path = item.FullName
                    });
                }
            }

            return models;
        }
    }
}