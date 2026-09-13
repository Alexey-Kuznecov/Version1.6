
using System;
using UnityCommander.Search.Models;

namespace UnityCommander.Modules.LeftSideBars.Search
{
    public sealed class SearchResultItemViewModel
    {
        public SearchItem Item { get; }

        public string Path => Item.Path;
        public string? Name => Item.Name;

        public bool IsDirectory => Item.IsDirectory;

        public long Size => Item.Size;

        public DateTime LastWriteTime => Item.LastWriteTime;

        public string DisplaySize =>
            IsDirectory
                ? string.Empty
                : FormatSize(Size);

        public string DisplayDate =>
            LastWriteTime.ToString("dd.MM.yyyy HH:mm");

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

        public SearchResultItemViewModel(SearchItem item)
        {
            Item = item;
        }
    }
}
