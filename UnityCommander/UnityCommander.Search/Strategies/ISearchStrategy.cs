
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Strategies
{
    public interface ISearchStrategy
    {
        IAsyncEnumerable<SearchResult> SearchAsync(
            IEnumerable<SearchItem> items,
            SearchRequest request,
            CancellationToken cancellationToken = default);
    }
}
