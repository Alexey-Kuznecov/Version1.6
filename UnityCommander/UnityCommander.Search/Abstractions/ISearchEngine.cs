
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Abstractions
{
    public interface ISearchEngine
    {
        IAsyncEnumerable<SearchResult> SearchAsync(
            SearchRequest request,
            CancellationToken cancellationToken = default);
    }
}
