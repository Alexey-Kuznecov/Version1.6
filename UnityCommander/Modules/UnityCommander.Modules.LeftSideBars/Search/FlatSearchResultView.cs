
using System.Collections.Generic;
using UnityCommander.Modules.LeftSideBars.Models;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class FlatSearchResultView : ISearchResultView
    {
        public SearchResultViewMode Mode =>
            SearchResultViewMode.Flat;

        public IReadOnlyList<SearchResultItemViewModel> Items { get; }

        public FlatSearchResultView(
            IReadOnlyList<SearchResultItemViewModel> items)
        {
            Items = items;
        }
    }
}
