
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UnityCommander.Common.Panels;
using UnityCommander.Modules.LeftSideBars.Models;
using UnityCommander.Modules.LeftSideBars.Search;
using UnityCommander.Modules.LeftSideBars.Services;
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
        private readonly ISearchResultBuilder _resultBuilder;

        private readonly ISearchService _searchService;

        private readonly ActiveTabContext _activeTabContext;

        private CancellationTokenSource? _searchCancellation;

        public SearchControlViewModel(
            ISearchResultBuilder resultBuilder,
            ISearchService searchService,
            ActiveTabContext activeTab)
        {
            _resultBuilder = resultBuilder;
            _searchService = searchService;
            _activeTabContext = activeTab;

            SearchCommand = new RelayCommand(
                _ => ExecuteSearch());

            CancelSearchCommand = new RelayCommand(
               _ => CancelSearch(),
               _ => IsSearching);

            ShowListCommand = new RelayCommand(
                _ => ViewMode = SearchResultViewMode.Flat);

            ShowFoldersCommand = new RelayCommand(
                _ => ViewMode = SearchResultViewMode.Folders);

            ExpandAllCommand = new RelayCommand(
                _ => ExpandAll());

            CollapseAllCommand = new RelayCommand(
                _ => CollapseAll());
        }

        private void ExpandAll()
        {
            if (Results is not FolderSearchResultView folders)
                return;

            foreach (var folder in folders.Folders)
                folder.IsExpanded = true;
        }

        private void CollapseAll()
        {
            if (Results is not FolderSearchResultView folders)
                return;

            foreach (var folder in folders.Folders)
                folder.IsExpanded = false;
        }

        private void CancelSearch()
        {
            _searchCancellation?.Cancel();
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

            IsSearching = true;
            ResultCount = 0;

            var progress = new Progress<SearchProgress>(
                value => Progress = value);

            var request = BuildRequest(path, progress);

            try
            {
                var items = new List<SearchItem>();

                await foreach (var result in _searchService.Search(
                    request,
                    _searchCancellation.Token))
                {
                    if (result is not FileSearchResult fileResult)
                        continue;

                    items.Add(fileResult.Item);
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _searchItems.Clear();
                    _searchItems.AddRange(items);

                    ResultCount = items.Count;

                    RebuildResults(path);
                });
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    IsSearching = false;
                });
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

            // ---------------------------------------------------------
            // Extension
            // ---------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(Extension))
            {
                filters.Add(
                    new ExtensionSearchFilter(
                        [Extension]));
            }

            // ---------------------------------------------------------
            // Modified
            // ---------------------------------------------------------

            switch (ModifiedFilter)
            {
                case ModifiedFilterMode.Today:
                    filters.Add(
                        new DateSearchFilter(
                            DateField.LastWrite,
                            DateComparison.After,
                            DateTime.Today));
                    break;

                case ModifiedFilterMode.Yesterday:
                    var yesterday = DateTime.Today.AddDays(-1);

                    filters.Add(
                        new DateSearchFilter(
                            DateField.LastWrite,
                            DateComparison.After,
                            yesterday));

                    filters.Add(
                        new DateSearchFilter(
                            DateField.LastWrite,
                            DateComparison.Before,
                            DateTime.Today));

                    break;

                case ModifiedFilterMode.Last7Days:
                    filters.Add(
                        new DateSearchFilter(
                            DateField.LastWrite,
                            DateComparison.After,
                            DateTime.Today.AddDays(-7)));
                    break;

                case ModifiedFilterMode.Last30Days:
                    filters.Add(
                        new DateSearchFilter(
                            DateField.LastWrite,
                            DateComparison.After,
                            DateTime.Today.AddDays(-30)));
                    break;

                case ModifiedFilterMode.Any:
                case ModifiedFilterMode.Custom:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // ---------------------------------------------------------
            // Size
            // ---------------------------------------------------------

            switch (SizeFilter)
            {
                case SizeFilterMode.Empty:

                    filters.Add(
                        new SizeSearchFilter(
                            SizeComparison.Equal,
                            0));

                    break;

                case SizeFilterMode.Small:

                    filters.Add(
                        new SizeSearchFilter(
                            SizeComparison.LessThan,
                            1024 * 1024));

                    break;

                case SizeFilterMode.Medium:

                    filters.Add(
                        new SizeSearchFilter(
                            SizeComparison.GreaterThanOrEqual,
                            1024 * 1024));

                    filters.Add(
                        new SizeSearchFilter(
                            SizeComparison.LessThan,
                            100 * 1024 * 1024));

                    break;

                case SizeFilterMode.Large:

                    filters.Add(
                        new SizeSearchFilter(
                            SizeComparison.GreaterThanOrEqual,
                            100 * 1024 * 1024));

                    break;

                case SizeFilterMode.Any:
                case SizeFilterMode.Custom:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

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

        private void RebuildResults(string? rootPath = null)
        {
            Results = _resultBuilder.Build(
                _searchItems,
                ViewMode,
                TreeScope,
                rootPath, 
                _searchText);
        }

        public ICommand SearchCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CancelSearchCommand { get; }
    }
}
