
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using UnityCommander.Business;

    /// <summary>
    /// The FilesProvider interface.
    /// </summary>
    public interface IFilesProvider
    {
        /// <summary>
        /// The get files.
        /// </summary>
        /// <returns> The List. </returns>
        List<FileModel> GetFiles();
    }
}
