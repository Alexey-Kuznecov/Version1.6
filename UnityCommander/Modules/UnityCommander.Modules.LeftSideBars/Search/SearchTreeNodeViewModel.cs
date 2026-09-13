
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchTreeNodeViewModel
    {
        public string Path { get; }

        public string Name { get; }

        public IReadOnlyList<SearchTreeNodeViewModel> Children { get; }

        public IReadOnlyList<SearchResultItemViewModel> Items { get; }

        public bool IsExpanded { get; set; }

        public int ResultCount =>
            Items.Count + Children.Sum(x => x.ResultCount);

        public SearchTreeNodeViewModel(
            string path,
            string name,
            IReadOnlyList<SearchTreeNodeViewModel> children,
            IReadOnlyList<SearchResultItemViewModel> items)
        {
            Path = path;
            Name = name;
            Children = children;
            Items = items;
        }
    }
}
