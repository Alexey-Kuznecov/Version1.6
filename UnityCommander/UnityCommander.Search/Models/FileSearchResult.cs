
namespace UnityCommander.Search.Models
{
    public sealed class FileSearchResult : SearchResult
    {
        public SearchItem Item { get; }

        public FileSearchResult(SearchItem item)
        {
            Item = item;
        }
    }
}
