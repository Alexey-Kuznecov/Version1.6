
using System.Collections.Generic;
using UnityCommander.Modules.LeftSideBars.Models;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class TreeSearchResultView : ISearchResultView
    {
        public SearchResultViewMode Mode =>
            SearchResultViewMode.Tree;

        public IReadOnlyList<SearchTreeNodeViewModel> Roots { get; }

        public TreeSearchResultView(
            IReadOnlyList<SearchTreeNodeViewModel> roots)
        {
            Roots = roots;
        }
    }
}
