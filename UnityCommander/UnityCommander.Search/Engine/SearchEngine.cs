
using System.Diagnostics;
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

            var processed = 0;
            var skipped = 0;
            var found = 0;

            await foreach (var item in _enumerator.EnumerateAsync(
                request.Scope,
                cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                processed++;

                if (request.Filters.Any(filter => !filter.Match(item)))
                {
                    skipped++;
                    ReportProgress();
                    continue;
                }

                if (request.Matcher != null &&
                    !request.Matcher.Match(item, request.Query ?? string.Empty))
                {
                    skipped++;
                    ReportProgress();
                    continue;
                }

                found++;

                ReportProgress();

                yield return new FileSearchResult(item);
            }

            void ReportProgress()
            {
                request.Progress?.Report(new SearchProgress
                {
                    Processed = processed,
                    Found = found,
                    Skipped = skipped
                });
            }
        }
    }
}
