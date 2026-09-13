
using System.Collections.Generic;
using UnityCommander.Modules.LeftSideBars.Models;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchResultView2
    {
        public SearchResultViewMode Mode { get; }

        public IReadOnlyList<SearchResultItemViewModel> ItemsList { get; }

        public IReadOnlyList<SearchFolderViewModel> FoldersList { get; }

        public IReadOnlyList<SearchTreeNodeViewModel> TreeList { get; }

        private SearchResultView2(
            SearchResultViewMode mode,
            IReadOnlyList<SearchResultItemViewModel> items,
            IReadOnlyList<SearchFolderViewModel> folders,
            IReadOnlyList<SearchTreeNodeViewModel> tree)
        {
            Mode = mode;
            ItemsList = items;
            FoldersList = folders;
            TreeList = tree;
        }

        public static SearchResultView2 Flat(
            IReadOnlyList<SearchResultItemViewModel> items)
            => new(
                SearchResultViewMode.Flat,
                items,
                [],
                []);

        public static SearchResultView2 Folder(
            IReadOnlyList<SearchFolderViewModel> folders)
            => new(
                SearchResultViewMode.Folders,
                [],
                folders,
                []);

        public static SearchResultView2 Tree(
            IReadOnlyList<SearchTreeNodeViewModel> tree)
            => new(
                SearchResultViewMode.Tree,
                [],
                [],
                tree);
    }
}
