using System.Collections.Generic;
using System.IO;
using UnityCommander.Business;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class FilesProvider : IFilesProvider
    {
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