
using System.Runtime.CompilerServices;
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Enumeration;
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Engine
{
    public sealed class SearchEngine : ISearchEngine
    {
        private readonly ISearchEnumerator _enumerator;

        public SearchEngine(ISearchEnumerator enumerator)
        {
            _enumerator = enumerator;
        }

        public async IAsyncEnumerable<SearchResult> SearchAsync(
            SearchRequest request,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            await foreach (var item in _enumerator.EnumerateAsync(
                request.Scope,
                cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Пока без фильтров и стратегии.
                // Просто рабочий проход через Enumerator.
                yield return new FileSearchResult(item);
            }
        }
    }
}
