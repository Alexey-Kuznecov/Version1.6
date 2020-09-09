
namespace UnityCommander.Services
{
    using System.Collections.Generic;
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
        public List<FileModel> GetFiles()
        {
            List<FileModel> models = new List<FileModel>();
            DirectoryInfo dir = new DirectoryInfo("c:\\");

            foreach (var item in dir.GetDirectories())
            {
                models.Add(new FileModel { FileName = item.Name });
            }

            return models;
        }
    }
}