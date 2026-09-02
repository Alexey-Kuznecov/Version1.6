
using System.IO;

namespace UnityCommander.Index.Models
{
    public sealed class IndexedFile
    {
        public long Id { get; internal set; }

        public long? ParentId { get; internal set; }

        public string Path { get; init; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Extension { get; init; } = string.Empty;

        public bool IsDirectory { get; init; }

        public long Size { get; init; }

        public DateTime CreationTime { get; init; }

        public DateTime LastWriteTime { get; init; }

        public DateTime LastAccessTime { get; init; }

        public FileAttributes Attributes { get; init; }
    }
}
