
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Abstractions
{
    public interface ISearchMatcher
    {
        bool Match(SearchItem item, string query);
    }
}
