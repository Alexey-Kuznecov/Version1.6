
namespace UnityCommander.Preview.Abstraction.Models;

public sealed class PreviewMetadata
{
    public string? Name { get; }

    public string? ContentType { get; }

    public long? Size { get; }

    public string? Description { get; }

    public PreviewMetadata(
        string? name = null,
        string? contentType = null,
        long? size = null,
        string? description = null)
    {
        Name = name;
        ContentType = contentType;
        Size = size;
        Description = description;
    }
}

