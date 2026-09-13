
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Automation;
using UnityCommander.Common.Models;
using UnityCommander.Modules.LeftSideBars.Models;
using UnityCommander.Modules.LeftSideBars.Search;
using UnityCommander.Mvvm.Base;
using UnityCommander.Search.Models;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public partial class SearchControlViewModel : PropertiesChanged
    { 
        // ---------------------------------------------------------
        // Query
        // ---------------------------------------------------------
        private string _searchText = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private SearchMatcherMode _searchMode = SearchMatcherMode.Contains;

        public SearchMatcherMode SearchMode
        {
            get => _searchMode;
            set => SetProperty(ref _searchMode, value);
        }

        private bool _isCaseSensitive;

        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set => SetProperty(ref _isCaseSensitive, value);
        }

        // ---------------------------------------------------------
        // Scope
        // ---------------------------------------------------------

        public SearchScopeMode SearchScope
        {
            get => _searchScope;
            set => SetProperty(ref _searchScope, value);
        }

        private SearchScopeMode _searchScope =
            SearchScopeMode.CurrentFolder;


        //// ---------------------------------------------------------
        //// Type
        //// ---------------------------------------------------------

        public SearchItemType FileType
        {
            get => _fileType;
            set => SetProperty(ref _fileType, value);
        }

        private SearchItemType _fileType =
            SearchItemType.All;

        private string _extension = string.Empty;

        public string Extension
        {
            get => _extension;
            set => SetProperty(ref _extension, value);
        }

        // ---------------------------------------------------------
        // Date
        // ---------------------------------------------------------

        public ModifiedFilterMode ModifiedFilter
        {
            get => _modifiedFilter;
            set => SetProperty(ref _modifiedFilter, value);
        }

        private ModifiedFilterMode _modifiedFilter =
            ModifiedFilterMode.Any;

        // ---------------------------------------------------------
        // Size
        // ---------------------------------------------------------

        public SizeFilterMode SizeFilter
        {
            get => _sizeFilter;
            set => SetProperty(ref _sizeFilter, value);
        }

        private SizeFilterMode _sizeFilter =
            SizeFilterMode.Any;

        // ---------------------------------------------------------
        // Attributes
        // ---------------------------------------------------------

        private bool _includeHidden;

        public bool IncludeHidden
        {
            get => _includeHidden;
            set => SetProperty(ref _includeHidden, value);
        }

        private bool _includeSystem;

        public bool IncludeSystem
        {
            get => _includeSystem;
            set => SetProperty(ref _includeSystem, value);
        }
 
        private bool _includeFolders;

        public bool IncludeFolders
        {
            get => _includeFolders;
            set => SetProperty(ref _includeFolders, value);
        }

        // ---------------------------------------------------------
        // Results
        // ---------------------------------------------------------

        public ObservableCollection<SearchResultItemViewModel> Results { get; } = [];

        private long _resultCount;

        public long ResultCount
        {
            get => _resultCount;
            private set => SetProperty(ref _resultCount, value);
        }

        private IReadOnlyList<SearchItem> _searchItems = [];

        private ISearchResultView _results;

        // ---------------------------------------------------------
        // State
        // ---------------------------------------------------------

        private bool _isSearching;

        public bool IsSearching
        {
            get => _isSearching;
            private set => SetProperty(ref _isSearching, value);
        }

        private SearchProgress _progress;
     
        public SearchProgress Progress
        {
            get => _progress;
            private set => SetProperty(ref _progress, value);
        }
    }
}
