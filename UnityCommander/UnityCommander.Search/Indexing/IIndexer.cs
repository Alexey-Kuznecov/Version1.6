
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Indexing
{
    public interface IIndexer
    {
        Task IndexAsync(
            SearchItem item,
            CancellationToken cancellationToken = default);
    }
}
