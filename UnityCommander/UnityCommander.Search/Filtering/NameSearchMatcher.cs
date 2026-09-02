
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public sealed class NameSearchMatcher : ISearchMatcher
    {
        public bool Match(SearchItem item, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return true;

            return item.Name?.Contains(
                query,
                StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
