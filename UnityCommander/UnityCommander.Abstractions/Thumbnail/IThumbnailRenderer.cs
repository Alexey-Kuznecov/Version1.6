
namespace UnityCommander.Abstractions.Thumbnail
{
    public interface IThumbnailRenderer
    {
        Task<ThumbnailResult?> RenderAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token);
    }
}
