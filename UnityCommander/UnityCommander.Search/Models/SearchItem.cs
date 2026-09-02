
namespace UnityCommander.Search.Models
{
    public sealed class SearchItem
    {
        public string Path { get; }
        public string? Name { get; set; }

        public bool IsDirectory { get; init; }

        public DateTime CreationTime { get; init; }
        public DateTime LastWriteTime { get; init; }

        public long Size { get; init; }

        public SearchItem(string path, string? name = null)
        {
            Path = path;
            Name = name;
        }
    }
}
