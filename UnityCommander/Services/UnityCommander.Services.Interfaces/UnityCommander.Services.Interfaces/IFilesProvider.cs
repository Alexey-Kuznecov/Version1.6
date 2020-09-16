
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.ObjectModel;
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
        ObservableCollection<FileModel> GetFiles(string path);
    }
}
