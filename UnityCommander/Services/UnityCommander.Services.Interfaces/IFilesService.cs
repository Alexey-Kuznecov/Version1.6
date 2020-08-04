using System.Collections.Generic;
using System.IO;

namespace UnityCommander.Services.Interfaces
{
    public interface IFilesService
    {
        List<string> GetFiles();
    }
}
