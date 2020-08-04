using System.Collections.Generic;
using UnityCommander.Business;

namespace UnityCommander.Services.DataProvider.Interfaces
{
    public interface IFilesProvider
    {
        List<FileModel> GetFiles();
    }
}
