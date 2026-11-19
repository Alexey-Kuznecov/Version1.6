
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;
using UnityCommander.Preview.Abstraction.Models.Content;

namespace UnityCommander.Preview.Providers.Images;

public sealed class ImagePreviewProvider : IPreviewProvider
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp",
        ".gif",
        ".tif",
        ".tiff"
        };

    public string Id => "core.images";

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

        var stream = new FileStream(
            request.Path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);

        try
        {
            var contentType = GetContentType(request.Extension);

            var metadata = new PreviewMetadata(
                request.Name,
                contentType,
                stream.Length);

            var content = new ImagePreviewContent(
                stream,
                contentType);

            var result = new PreviewResult(
                content,
                metadata,
                PreviewCapabilities.Preview);

            return Task.FromResult(result);
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    private static string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".gif" => "image/gif",
            ".tif" or ".tiff" => "image/tiff",
            _ => "application/octet-stream"
        };
    }
}

