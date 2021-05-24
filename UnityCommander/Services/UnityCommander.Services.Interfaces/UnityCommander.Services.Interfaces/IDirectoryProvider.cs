
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.ObjectModel;
    using UnityCommander.Common.Models;

    /// <summary>
    /// The FilesProvider interface.
    /// </summary>
    public interface IDirectoryProvider
    {
        /// <summary>
        /// Gets list files to the specific path.
        /// </summary>
        /// <param name="path"> The path to the file location. </param>
        /// <returns> The collection of <see cref="FileModel"/> objects. </returns>
        ObservableCollection<FileModel> GetFiles(string path);

        /// <summary>
        /// Gets list directories to the specific path.
        /// </summary>
        /// <param name="path"> The path to the directories location. </param>
        /// <returns> The collection <see cref="FolderModel"/> objects. </returns>
        ObservableCollection<FolderModel> GetDirectories(string path);
    }
}
