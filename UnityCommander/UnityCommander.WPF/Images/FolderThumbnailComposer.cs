
using System.Windows;
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
            var count = Math.Min(thumbnails.Count, 4);

            if (count == 0)
                throw new ArgumentException(
                    "At least one thumbnail is required.",
                    nameof(thumbnails));

            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            var canvas = new byte[size * size * 4];
            var tileSize = size / 2;

            for (var i = 0; i < count; i++)
            {
                var source = ThumbnailImageCodec.Decode(thumbnails[i]);

                var cropped = ResizeAndCrop(
                    source,
                    tileSize,
                    tileSize);

                var pixels = ThumbnailImageCodec.ToBgra32(cropped);

                var stride = pixels.PixelWidth * 4;
                var sourceBytes = new byte[stride * pixels.PixelHeight];

                pixels.CopyPixels(sourceBytes, stride, 0);

                var tileX = (i % 2) * tileSize;
                var tileY = (i / 2) * tileSize;

                CopyPixels(
                    sourceBytes,
                    pixels.PixelWidth,
                    pixels.PixelHeight,
                    stride,
                    canvas,
                    size * 4,
                    tileX,
                    tileY);
            }

            var result = BitmapSource.Create(
                size,
                size,
                96,
                96,
                PixelFormats.Pbgra32,
                null,
                canvas,
                size * 4);

            result.Freeze();

            return ThumbnailImageCodec.Encode(result);
        }

        private static BitmapSource ResizeAndCrop(
            BitmapSource source,
            int width,
            int height)
        {
            var scaleX = width / (double)source.PixelWidth;
            var scaleY = height / (double)source.PixelHeight;

            // Масштабируем так, чтобы полностью заполнить область.
            var scale = Math.Max(scaleX, scaleY);

            var resized = new TransformedBitmap(
                source,
                new ScaleTransform(scale, scale));

            resized.Freeze();

            // Центральное кадрирование до нужного размера.
            var cropX = (resized.PixelWidth - width) / 2;
            var cropY = (resized.PixelHeight - height) / 2;

            var cropped = new CroppedBitmap(
                resized,
                new Int32Rect(
                    cropX,
                    cropY,
                    width,
                    height));

            cropped.Freeze();

            return cropped;
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
