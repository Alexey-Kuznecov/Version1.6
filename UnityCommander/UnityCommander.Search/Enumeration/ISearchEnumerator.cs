
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Enumeration
{
    public interface ISearchEnumerator
    {
        IAsyncEnumerable<SearchItem> EnumerateAsync(
            SearchScope scope,
            CancellationToken cancellationToken = default);
    }
}
