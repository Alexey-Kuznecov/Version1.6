
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityCommander.Modules.LeftSideBars.Models;
using UnityCommander.Modules.LeftSideBars.Search;
using UnityCommander.Search.Models;

namespace UnityCommander.Modules.LeftSideBars.Services
{
    public sealed class SearchResultBuilder : ISearchResultBuilder
    {
        public ISearchResultView Build(
            IReadOnlyList<SearchItem> items,
            SearchResultViewMode mode,
            SearchTreeScope treeScope,
            string? rootPath, 
            string? query)
        {
            return mode switch
            {
                SearchResultViewMode.Flat =>
                    BuildFlat(items, query),

                SearchResultViewMode.Folders =>
                    BuildFolders(items, query),

                SearchResultViewMode.Tree =>
                    BuildTree(items, treeScope, rootPath, query),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private static FlatSearchResultView BuildFlat(
            IReadOnlyList<SearchItem> items, 
            string? query)
        {
            var result = items
                .Select(x => new SearchResultItemViewModel(x, query))
                .ToList();

            return new FlatSearchResultView(result);
        }

        private static ISearchResultView BuildFolders(
            IReadOnlyList<SearchItem> items, 
            string? query)
        {
            var folders = items
                .GroupBy(GetDirectoryPath, StringComparer.OrdinalIgnoreCase)
                .Select(group => new SearchFolderViewModel(
                    group.Key,
                    GetDirectoryName(group.Key),
                    group.Select(x => new SearchResultItemViewModel(x, query)).ToList()))
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new FolderSearchResultView(folders);
        }

        private static ISearchResultView BuildTree(
            IReadOnlyList<SearchItem> items,
            SearchTreeScope scope,
            string? rootPath,
            string? query)
        {
            var resultItems = items
                .Select(x => new SearchResultItemViewModel(x, query))
                .ToList();

            var roots = BuildTreeNodes(
               resultItems,
               scope,
               rootPath, 
               query);

            return new TreeSearchResultView(roots);
        }

        private static string GetDirectoryPath(SearchItem item)
        {
            return item.IsDirectory
                ? item.Path
                : Path.GetDirectoryName(item.Path) ?? string.Empty;
        }

        private static string GetDirectoryName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return Path.GetFileName(
                       path.TrimEnd(Path.DirectorySeparatorChar))
                   ?? path;
        }

        private static IReadOnlyList<SearchTreeNodeViewModel> BuildTreeNodes(
            IReadOnlyList<SearchResultItemViewModel> items,
            SearchTreeScope scope,
            string? rootPath,
            string? query)
        {
            var root = new SearchTreeBuildNode(string.Empty, string.Empty);

            foreach (var item in items)
            {
                var directory = item.IsDirectory
                    ? item.Path
                    : Path.GetDirectoryName(item.Path);

                if (string.IsNullOrWhiteSpace(directory))
                    continue;

                AddItem(
                    root,
                    directory,
                    item,
                    scope,
                    rootPath);
            }

            return root.Children.Values
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => ToViewModel(x, query))
                .ToList();
        }

        private static void AddItem(
             SearchTreeBuildNode root,
             string directory,
             SearchResultItemViewModel item,
             SearchTreeScope scope,
             string? rootPath)
        {
            var parts = GetPathParts(
                directory,
                scope,
                rootPath);

            var current = root;

            foreach (var part in parts)
            {
                var path = string.IsNullOrEmpty(current.Path)
                    ? part
                    : Path.Combine(current.Path, part);

                if (!current.Children.TryGetValue(
                        part,
                        out var child))
                {
                    child = new SearchTreeBuildNode(path, part);
                    current.Children.Add(part, child);
                }

                current = child;
            }

            current.Items.Add(item);
        }

        private static SearchTreeNodeViewModel ToViewModel(
          SearchTreeBuildNode node,
          string? query)
        {
            var children = node.Children.Values
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .Select(x => ToViewModel(x, query))
                .ToList();

            var items = node.Items
                .OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new SearchTreeNodeViewModel(
                node.Path,
                node.Name,
                children,
                items,
                query,
                isExpanded: true);
        }

        private static IReadOnlyList<string> GetPathParts(
           string directory,
           SearchTreeScope scope,
           string? rootPath)
        {
            var path = directory;

            if (scope == SearchTreeScope.ResultFoldersOnly &&
                !string.IsNullOrWhiteSpace(rootPath))
            {
                path = Path.GetRelativePath(
                    rootPath,
                    directory);
            }

            return path.Split(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar,
                StringSplitOptions.RemoveEmptyEntries);
        }

        private sealed class SearchTreeBuildNode
        {
            public string Path { get; }

            public string Name { get; }

            public Dictionary<string, SearchTreeBuildNode> Children { get; }
                = new(StringComparer.OrdinalIgnoreCase);

            public List<SearchResultItemViewModel> Items { get; } = [];

            public SearchTreeBuildNode(string path, string name)
            {
                Path = path;
                Name = name;
            }
        }
    }
}
