using System;
using System.Collections.Generic;
using System.IO;
using UnityCommander.Business;
using UnityCommander.Services.DataProvider.Interfaces;

namespace UnityCommander.Services.DataProvider
{
    public class FilesProvider : IFilesProvider
    {
        List<FileModel> IFilesProvider.GetFiles()
        {
            List<FileModel> models = new List<FileModel>();
            DirectoryInfo dir = new DirectoryInfo("c:\\");

            foreach (var item in dir.GetDirectories())
            {
                models.Add(new FileModel() { FileName = item.Name });
            }

            return models;
        }
    }
}
