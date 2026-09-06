
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Filtering;
using UnityCommander.Search.Strategies;

namespace UnityCommander.Search.Models
{
    public sealed class SearchRequest
    {
        public SearchScope Scope { get; init; }

        public string? Query { get; init; }

        public IReadOnlyList<ISearchFilter> Filters { get; init; }
            = [];

        public ISearchMatcher? Matcher { get; init; }

        public ISearchStrategy? Strategy { get; init; }

        public IProgress<SearchProgress>? Progress { get; set; }
    }
}
