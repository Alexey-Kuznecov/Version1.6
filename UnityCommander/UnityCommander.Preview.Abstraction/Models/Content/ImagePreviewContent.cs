
namespace UnityCommander.Preview.Abstraction.Models.Content;

public sealed class ImagePreviewContent : PreviewContent
{
    private readonly bool _leaveOpen;

    public Stream Data { get; }

    public string? ContentType { get; }

    public int? Width { get; }

    public int? Height { get; }

    public ImagePreviewContent(
        Stream data,
        string? contentType = null,
        int? width = null,
        int? height = null,
        bool leaveOpen = false)
        : base(PreviewKind.Image)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        ContentType = contentType;
        Width = width;
        Height = height;
        _leaveOpen = leaveOpen;
    }

    public override void Dispose()
    {
        if (!_leaveOpen)
            Data.Dispose();
    }
}