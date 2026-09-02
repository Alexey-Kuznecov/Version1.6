using UnityCommander.Index.Models;

namespace UnityCommander.Index.Abstractions
{
    public interface IFileIndexReader
    {
        Task<IndexedFile?> GetAsync(
            long id,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<IndexedFile> EnumerateChildrenAsync(
            long? parentId,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<IndexedFile> EnumerateAsync(
            CancellationToken cancellationToken = default);
    }
}
