
namespace UnityCommander.Preview.Abstraction.Models.Content;

public sealed class DocumentPreviewContent : PreviewContent
{
    private readonly bool _leaveOpen;

    public Stream Data { get; }

    public string? ContentType { get; }

    public DocumentPreviewContent(
        Stream data,
        string? contentType = null,
        bool leaveOpen = false)
        : base(PreviewKind.Document)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        ContentType = contentType;
        _leaveOpen = leaveOpen;
    }

    public override void Dispose()
    {
        if (!_leaveOpen)
            Data.Dispose();
    }
}
