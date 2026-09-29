
namespace UnityCommander.Abstractions.Thumbnail
{
    public interface IShellThumbnailExtractor
    {
        Task<ThumbnailResult?> ExtractAsync(
            string path,
            int size,
            CancellationToken token);
    }
}
