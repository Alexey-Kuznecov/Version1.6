
namespace UnityCommander.Abstractions.Thumbnail
{
    public interface IThumbnailService
    {
        Task<ThumbnailResult?> GetAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token);
    }
}
