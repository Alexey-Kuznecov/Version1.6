
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Filtering
{
    public interface ISearchFilter
    {
        bool Match(SearchItem item);
    }
}
