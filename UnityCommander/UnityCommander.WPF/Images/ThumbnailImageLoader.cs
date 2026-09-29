
using System.Windows.Media;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class ThumbnailImageLoader
    {
        private readonly IThumbnailService _service;

        public ThumbnailImageLoader(
            IThumbnailService service)
        {
            _service = service;
        }

        public async Task<ImageSource?> LoadAsync(
            string path,
            int size,
            CancellationToken token)
        {
            var result =
                await _service.GetAsync(
                    path,
                    new ThumbnailRequest(size),
                    token);

            if (result is null)
                return null;

            return ThumbnailImageCodec.Decode(
                result);
        }
    }
}
