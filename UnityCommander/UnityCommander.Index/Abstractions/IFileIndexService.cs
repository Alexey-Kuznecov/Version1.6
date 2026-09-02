
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

        Task<long> AddAsync(
           IndexedFile file,
           CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<IndexedFile> files,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            IndexedFile file,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            long id,
            CancellationToken cancellationToken = default);
    }
}
