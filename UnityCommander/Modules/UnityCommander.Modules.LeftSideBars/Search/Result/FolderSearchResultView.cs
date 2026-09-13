
using System.Collections.Generic;
using UnityCommander.Modules.LeftSideBars.Models;

namespace UnityCommander.Modules.LeftSideBars.Search.Result
{
    public sealed class FolderSearchResultView : ISearchResultView
    {
        public SearchResultViewMode Mode =>
            SearchResultViewMode.Folders;

        public IReadOnlyList<SearchFolderViewModel> Folders { get; }

        public FolderSearchResultView(
            IReadOnlyList<SearchFolderViewModel> folders)
        {
            Folders = folders;
        }
    }
}
