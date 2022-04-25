
namespace AIconBrowser.Contracts
{
    /// <summary>
    /// The FileService interface.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object Open(string path);

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="obj">
        /// The obj.
        /// </param>
        void Save(string path, object obj);
    }
}
