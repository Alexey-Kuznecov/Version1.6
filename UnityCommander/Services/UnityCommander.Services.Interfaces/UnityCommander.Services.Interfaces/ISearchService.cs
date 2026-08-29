
using System.Collections.Generic;
using System.Threading;
using UnityCommander.Search.Models;

namespace UnityCommander.Services.Interfaces
{
    public interface ISearchService
    {
        IAsyncEnumerable<SearchResult> Search(
            SearchRequest request,
            CancellationToken cancellationToken = default);
    }
}
