
using System.Collections.Generic;
using System.Threading;
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Models;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class SearchService : ISearchService
    {
        private readonly ISearchEngine _engine;

        public SearchService(ISearchEngine engine)
        {
            _engine = engine;
        }

        public IAsyncEnumerable<SearchResult> Search(
            SearchRequest request,
            CancellationToken cancellationToken = default)
            => _engine.SearchAsync(request, cancellationToken);
    }
}
