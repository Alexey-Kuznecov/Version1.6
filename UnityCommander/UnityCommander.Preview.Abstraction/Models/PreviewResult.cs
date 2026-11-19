
namespace UnityCommander.Preview.Abstraction.Models;

public sealed class PreviewResult : IDisposable
{
    public PreviewContent Content { get; }

    public PreviewMetadata Metadata { get; }

    public PreviewCapabilities Capabilities { get; }

    public string? PresenterId { get; }

    public PreviewResult(
        PreviewContent content,
        PreviewMetadata? metadata = null,
        PreviewCapabilities capabilities = PreviewCapabilities.Preview,
        string? presenterId = null)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Metadata = metadata ?? new PreviewMetadata();
        Capabilities = capabilities;
        PresenterId = presenterId;
    }

    public void Dispose()
    {
        Content.Dispose();
    }
}
