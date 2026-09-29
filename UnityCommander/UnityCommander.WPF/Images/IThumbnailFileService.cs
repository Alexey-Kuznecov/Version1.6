
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public interface IThumbnailFileService
    {
        Task<ThumbnailResult?> GetAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token);
    }
}
