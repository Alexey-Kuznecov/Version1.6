
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Mvvm.Base;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchTreeNodeViewModel : PropertiesChanged
    {
        private bool _isExpanded;

        public string Path { get; }

        public string Name { get; }

        public string SearchQuery { get; }

        public IReadOnlyList<SearchTreeNodeViewModel> Children { get; }

        public IReadOnlyList<SearchResultItemViewModel> Items { get; }
        
        public IReadOnlyList<object> Entries { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public int ResultCount =>
            Items.Count + Children.Sum(x => x.ResultCount);

        public SearchTreeNodeViewModel(
            string path,
            string name,
            IReadOnlyList<SearchTreeNodeViewModel> children,
            IReadOnlyList<SearchResultItemViewModel> items,
            string query,
            bool isExpanded = false)
        {
            Path = path;
            Name = name;
            Children = children;
            Items = items;

            Entries = children
                 .Cast<object>()
                 .Concat(items)
                 .ToList();

            SearchQuery = query;
            IsExpanded = isExpanded;
        }
    }
}
