
using System;
using UnityCommander.Mvvm.Base;
using UnityCommander.Search.Models;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchResultItemViewModel : PropertiesChanged
    {
        public SearchItem Item { get; }

        public string Path => Item.Path;
        public string? Name => Item.Name;
        public bool IsDirectory => Item.IsDirectory;
        public long Size => Item.Size;
        public DateTime LastWriteTime => Item.LastWriteTime;

        public string SearchQuery { get; }

        public string DisplaySize =>
            IsDirectory
                ? string.Empty
                : FormatSize(Size);

        public string DisplayDate =>
            LastWriteTime.ToString("dd.MM.yyyy HH:mm");

        public SearchResultItemViewModel(
            SearchItem item,
            string searchQuery)
        {
            Item = item;
            SearchQuery = searchQuery;
        }

        private static string FormatSize(long size)
        {
            if (size < 1024)
                return $"{size} B";

            if (size < 1024 * 1024)
                return $"{size / 1024d:0.#} KB";

            if (size < 1024 * 1024 * 1024)
                return $"{size / (1024d * 1024):0.#} MB";

            return $"{size / (1024d * 1024 * 1024):0.#} GB";
        }
    }
}
