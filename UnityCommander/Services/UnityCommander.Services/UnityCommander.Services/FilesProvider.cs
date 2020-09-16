
namespace UnityCommander.Services
{
    using System.Collections.ObjectModel;
    using System.IO;

    using UnityCommander.Business;
    using UnityCommander.Services.Interfaces;

    /// <summary>
    /// The files provider.
    /// </summary>
    public class FilesProvider : IFilesProvider
    {
        /// <summary>
        /// The get files.
        /// </summary>
        /// <returns> The List. </returns>
        public ObservableCollection<FileModel> GetFiles(string path)
        {
            ObservableCollection<FileModel> models = new ObservableCollection<FileModel>();
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (var item in dir.GetDirectories())
            {
                models.Add(new FileModel 
                {
                    FileName = item.Name,
                    FullName = item.FullName,
                });
            }

            return models;
        }
    }
}