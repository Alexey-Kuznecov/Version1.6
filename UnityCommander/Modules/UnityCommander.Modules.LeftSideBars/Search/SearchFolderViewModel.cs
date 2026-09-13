
using System.Collections.Generic;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchFolderViewModel
    {
        public string Path { get; }

        public string Name { get; }

        public IReadOnlyList<SearchResultItemViewModel> Items { get; }

        public bool IsExpanded { get; set; }

        public SearchFolderViewModel(
            string path,
            string name,
            IReadOnlyList<SearchResultItemViewModel> items)
        {
            Path = path;
            Name = name;
            Items = items;
        }
    }
}
