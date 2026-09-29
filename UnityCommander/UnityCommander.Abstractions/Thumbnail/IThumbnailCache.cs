
namespace UnityCommander.Abstractions.Thumbnail
{
    public interface IThumbnailCache
    {
        int Priority { get; }

        Task<ThumbnailResult?> GetAsync(
            string key,
            CancellationToken token);

        Task SetAsync(
            string key,
            ThumbnailResult value,
            CancellationToken token);
    }
}
