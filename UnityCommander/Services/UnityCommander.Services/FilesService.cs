using System;
using System.Collections.Generic;
using System.IO;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    class FilesService : IFilesService
    {
        List<string> IFilesService.GetFiles()
        {
            DirectoryInfo dir = new DirectoryInfo("c:\\");

            foreach (var item in dir.GetDirectories())
            {

            }

            return null;
        }
    }
}
