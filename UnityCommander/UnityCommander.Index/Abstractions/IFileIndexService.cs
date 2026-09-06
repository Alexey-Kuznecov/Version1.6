
using UnityCommander.Index.Models;

namespace UnityCommander.Index.Abstractions
{
    public interface IFileIndexService
    {
        bool IsAvailable { get; }

        IAsyncEnumerable<IndexedFile> EnumerateAsync(
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<IndexedFile> EnumerateChildrenAsync(
            long? parentId,
            CancellationToken cancellationToken = default);

        Task<IndexedFile?> GetAsync(
            long id,
            CancellationToken cancellationToken = default);

        Task<IndexAddResult> AddAsync(
           string path,
           CancellationToken cancellationToken = default);

        Task<IndexOperationResult> AddRecursiveAsync(
            string path,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            IndexedFile file,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            long id,
            CancellationToken cancellationToken = default);
    }
}
