
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;
using UnityCommander.Preview.Abstraction.Models.Content;

namespace UnityCommander.Preview.Providers.Video;

public sealed class VideoPreviewProvider : IPreviewProvider
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".mp4",
            ".mkv",
            ".avi",
            ".mov",
            ".wmv",
            ".webm",
            ".m4v",
            ".mpeg",
            ".mpg"
        };

    public string Id => "core.videos";

    public int Priority => 100;

    public ValueTask<bool> CanPreviewAsync(
        PreviewRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.FromResult(
            SupportedExtensions.Contains(request.Extension));
    }

    public Task<PreviewResult> CreatePreviewAsync(
        PreviewRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var content = new VideoPreviewContent(
            new Uri(Path.GetFullPath(request.Path)));

        var metadata = new PreviewMetadata(
            request.Name,
            "video/*",
            0);

        var result = new PreviewResult(
            content,
            metadata,
            PreviewCapabilities.Preview);

        return Task.FromResult(result);
    }
}