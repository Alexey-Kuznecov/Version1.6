
using System.Collections.Generic;
using UnityCommander.Modules.LeftSideBars.Models;
using UnityCommander.Modules.LeftSideBars.Search;
using UnityCommander.Search.Models;

namespace UnityCommander.Modules.LeftSideBars.Services
{
    public interface ISearchResultBuilder
    {
        public ISearchResultView Build(
           IReadOnlyList<SearchItem> items,
           SearchResultViewMode mode,
           SearchTreeScope treeScope,
           string? rootPath,
           string? query);
    }
}
