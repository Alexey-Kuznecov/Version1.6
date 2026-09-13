
using UnityCommander.Search.Models;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchResultOpenContext
    {
        public string Path { get; }
        public string? ParentDirectory { get; }
        public bool IsDirectory { get; }

        public SearchResultOpenContext(SearchItem item)
        {
            Path = item.Path;
            IsDirectory = item.IsDirectory;

            ParentDirectory = System.IO.Path.GetDirectoryName(item.Path);
        }
    }
}
