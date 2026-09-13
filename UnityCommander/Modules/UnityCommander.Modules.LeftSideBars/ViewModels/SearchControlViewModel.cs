
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using UnityCommander.Common.Panels;
using UnityCommander.Modules.LeftSideBars.Models;
using UnityCommander.Modules.LeftSideBars.Search;
using UnityCommander.Mvvm;
using UnityCommander.Mvvm.Base;
using UnityCommander.Search.Abstractions;
using UnityCommander.Search.Filtering;
using UnityCommander.Search.Models;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public partial class SearchControlViewModel :  PropertiesChanged
    {
        private readonly ISearchService _searchService;

        private readonly ActiveTabContext _activeTabContext;

        private CancellationTokenSource? _searchCancellation;

        //private SearchMode _searchMode;
        //private SearchScopeOption _searchScope;

        public SearchControlViewModel(
            ISearchService searchService,
            ActiveTabContext activeTab)
        {
            _searchService = searchService;
            _activeTabContext = activeTab;

            SearchCommand = new RelayCommand(
                _ => ExecuteSearch());
        }

        private void ExecuteSearch()
        {
            _ = SearchAsync();
        }

        private async Task SearchAsync()
        {
            var path = _activeTabContext.CurrentPath;

            if (string.IsNullOrWhiteSpace(path))
                return;

            _searchCancellation?.Cancel();
            _searchCancellation?.Dispose();

            _searchCancellation = new CancellationTokenSource();

            Results.Clear();
            ResultCount = 0;
            IsSearching = true;

            var progress = new Progress<SearchProgress>(
                value => Progress = value);

            var request = BuildRequest(path, progress);

            try
            {
                await foreach (var result in _searchService.Search(
                    request,
                    _searchCancellation.Token))
                {
                    if (result is not FileSearchResult fileResult)
                        continue;

                    Results.Add(new SearchResultItemViewModel(fileResult.Item));
                    ResultCount++;
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                IsSearching = false;
            }
        }

        private SearchRequest BuildRequest(
            string path, 
            Progress<SearchProgress> progress)
        {
            return new SearchRequest
            {
                Scope = new SearchScope
                {
                    Paths = [path]
                },

                Query = SearchText,

                Filters = BuildFilters(),

                Matcher = BuildMatcher(),

                Progress = progress
            };
        }

        private IReadOnlyList<ISearchFilter> BuildFilters()
        {
            var filters = new List<ISearchFilter>();

            if (!string.IsNullOrWhiteSpace(Extension))
            {
                filters.Add(
                    new ExtensionSearchFilter(
                        [Extension]));
            }

            // modified...

            // size...

            return filters;
        }

        private ISearchMatcher BuildMatcher()
        {
            return SearchMode switch
            {
                SearchMatcherMode.Contains =>
                    new NameSearchMatcher(),

                SearchMatcherMode.Mask =>
                    new WildcardSearchMatcher(),

                //SearchMode.StartsWith =>
                //    new StartsWithSearchMatcher(),

                //SearchMode.Regex =>
                //    new RegexSearchMatcher(),

                _ => new NameSearchMatcher()
            };
        }
        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
    }
}
