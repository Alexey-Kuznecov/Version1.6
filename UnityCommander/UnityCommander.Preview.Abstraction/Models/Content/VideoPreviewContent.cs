
namespace UnityCommander.Preview.Abstraction.Models.Content;

public sealed class VideoPreviewContent : PreviewContent
{
    public Uri Source { get; }

    public VideoPreviewContent(Uri source)
        : base(PreviewKind.Video)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
    }
}
