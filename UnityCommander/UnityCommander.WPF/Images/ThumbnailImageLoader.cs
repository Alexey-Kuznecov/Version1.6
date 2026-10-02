
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class ThumbnailImageLoader
    {
        private readonly ConditionalWeakTable<
            ThumbnailResult,
            ImageSource> _cache = new();

        public ImageSource GetImage(ThumbnailResult thumbnail)
        {
            if (_cache.TryGetValue(thumbnail, out var image))
                return image;

            image = ThumbnailImageCodec.Decode(thumbnail);

            _cache.Add(thumbnail, image);

            return image;
        }
    }
}
