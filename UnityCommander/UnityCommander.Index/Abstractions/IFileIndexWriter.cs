
using UnityCommander.Index.Models;

namespace UnityCommander.Index.Abstractions
{
    public interface IFileIndexWriter
    {
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
