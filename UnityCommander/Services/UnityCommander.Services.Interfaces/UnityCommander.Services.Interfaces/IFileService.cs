using System.Collections.Generic;
using UnityCommander.Business;

namespace UnityCommander.Services.Interfaces
{
    public interface IFilesProvider
    {
        List<FileModel> GetFiles();
    }
}
