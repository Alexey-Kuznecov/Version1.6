
namespace UnityCommander.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityCommander.Common.Models.Directory;

    /// <summary>
    /// The FilesProvider interface.
    /// </summary>
    public interface IDataProviderService
    {
        /// <summary>
        /// Gets list files to the specific path.
        /// </summary>
        /// <param name="path"> The path to the file location. </param>
        /// <returns> The collection of <see cref="FileModel"/> objects. </returns>
        Task<List<FileModel>> GetFilesAsync(string path, CancellationToken cancellation);

        /// <summary>
        /// Gets list directories to the specific path.
        /// </summary>
        /// <param name="path"> The path to the directories location. </param>
        /// <returns> The collection <see cref="FolderModel"/> objects. </returns>
        Task<List<FolderModel>> GetDirectoriesAsync(string path, CancellationToken cancellation);

        /// <summary>
        /// Gets list disks and devices.
        /// </summary>
        /// <returns> The collection <see cref="DriveModel"/> objects. </returns>
        public Task<List<DriveModel>> GetDrivesAsync();
    }
}
