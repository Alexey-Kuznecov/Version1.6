
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.Preview.Abstraction.Models;

public sealed class FolderPreviewContent : PreviewContent
{
    public string Path { get; }

    public int FileCount { get; }

    public int DirectoryCount { get; }

    public long TotalSize { get; }

    public DateTime CreationTime { get; }

    public DateTime LastModified { get; }

    public IReadOnlyList<ThumbnailResult> Thumbnails { get; }

    public FolderPreviewContent(
        string path,
        int fileCount,
        int directoryCount,
        long totalSize,
        DateTime creationTime,
        DateTime lastModified,
        IReadOnlyList<ThumbnailResult> thumbnails)
        : base(PreviewKind.Folder)
    {
        Path = path;
        FileCount = fileCount;
        DirectoryCount = directoryCount;
        TotalSize = totalSize;
        CreationTime = creationTime;
        LastModified = lastModified;
        Thumbnails = thumbnails ?? Array.Empty<ThumbnailResult>();
    }
}