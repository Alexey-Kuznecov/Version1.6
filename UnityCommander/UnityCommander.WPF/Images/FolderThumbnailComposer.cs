
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public static class FolderThumbnailComposer
    {
        public static ThumbnailResult Compose(
            IReadOnlyList<ThumbnailResult> thumbnails,
            int size)
        {
            var count = Math.Min(
                thumbnails.Count,
                4);

            if (count == 0)
                throw new ArgumentException(
                    "At least one thumbnail is required.",
                    nameof(thumbnails));

            var canvas = new byte[size * size * 4];

            var tileSize = size / 2;

            for (var i = 0; i < count; i++)
            {
                var source =
                    ThumbnailImageCodec.Decode(
                        thumbnails[i]);

                var resized =
                    ResizeToFit(
                        source,
                        tileSize,
                        tileSize);

                var pixels =
                    ThumbnailImageCodec.ToBgra32(
                        resized);

                var stride =
                    pixels.PixelWidth * 4;

                var sourceBytes =
                    new byte[stride * pixels.PixelHeight];

                pixels.CopyPixels(
                    sourceBytes,
                    stride,
                    0);

                var tileX =
                    (i % 2) * tileSize;

                var tileY =
                    (i / 2) * tileSize;

                var offsetX =
                    tileX + (tileSize - pixels.PixelWidth) / 2;

                var offsetY =
                    tileY + (tileSize - pixels.PixelHeight) / 2;

                CopyPixels(
                    sourceBytes,
                    pixels.PixelWidth,
                    pixels.PixelHeight,
                    stride,
                    canvas,
                    size * 4,
                    offsetX,
                    offsetY);
            }

            var result =
                BitmapSource.Create(
                    size,
                    size,
                    96,
                    96,
                    PixelFormats.Pbgra32,
                    null,
                    canvas,
                    size * 4);

            result.Freeze();

            return ThumbnailImageCodec.Encode(
                result);
        }

        private static BitmapSource ResizeToFit(
            BitmapSource source,
            int width,
            int height)
        {
            var scaleX =
                width / (double)source.PixelWidth;

            var scaleY =
                height / (double)source.PixelHeight;

            var scale =
                Math.Min(scaleX, scaleY);

            var transform =
                new ScaleTransform(
                    scale,
                    scale);

            var bitmap =
                new TransformedBitmap(
                    source,
                    transform);

            bitmap.Freeze();

            return bitmap;
        }

        private static void CopyPixels(
            byte[] source,
            int width,
            int height,
            int sourceStride,
            byte[] destination,
            int destinationStride,
            int destinationX,
            int destinationY)
        {
            var rowBytes = width * 4;

            for (var y = 0; y < height; y++)
            {
                Buffer.BlockCopy(
                    source,
                    y * sourceStride,
                    destination,
                    (destinationY + y) * destinationStride +
                    destinationX * 4,
                    rowBytes);
            }
        }
    }
}
