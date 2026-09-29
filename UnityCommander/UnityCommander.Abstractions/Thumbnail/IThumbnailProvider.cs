
namespace UnityCommander.Abstractions.Thumbnail
{
    public interface IThumbnailProvider
    {
        string Id { get; }

        int Priority { get; }

        bool CanProvide(string path);

        ValueTask<string?> GetCacheKeyAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token);

        Task<ThumbnailResult?> GetAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token);
    }
}
