
namespace UnityCommander.Preview.Abstraction.Models;

public sealed class PreviewRequest
{
    public string Path { get; }

    public string Name { get; }

    public string Extension { get; }

    public string? ContentType { get; }

    public long? Size { get; }

    public DateTimeOffset? LastModified { get; }

    public PreviewRequest(
        string path,
        string? contentType = null,
        long? size = null,
        DateTimeOffset? lastModified = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty.", nameof(path));

        Path = path;
        Name = System.IO.Path.GetFileName(path);
        Extension = System.IO.Path.GetExtension(path);

        ContentType = contentType;
        Size = size;
        LastModified = lastModified;
    }
}
