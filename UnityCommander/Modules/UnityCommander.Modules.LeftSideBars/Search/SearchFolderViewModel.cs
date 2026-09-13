
using System.Collections.Generic;
using UnityCommander.Mvvm.Base;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchFolderViewModel : PropertiesChanged
    {
        public string Path { get; }

        public string Name { get; }

        public IReadOnlyList<SearchResultItemViewModel> Items { get; }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public SearchFolderViewModel(
            string path,
            string name,
            IReadOnlyList<SearchResultItemViewModel> items,
            bool isExpanded = false)
        {
            Path = path;
            Name = name;
            Items = items;
            IsExpanded = isExpanded;
        }
    }
}
