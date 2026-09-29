
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public static class ThumbnailImageCodec
    {
        public static BitmapSource Decode(
            ThumbnailResult thumbnail)
        {
            using var stream = new MemoryStream(
                thumbnail.PngData,
                writable: false);

            var decoder = BitmapDecoder.Create(
                stream,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            var frame = decoder.Frames[0];

            frame.Freeze();

            return frame;
        }

        public static ThumbnailResult Encode(
            BitmapSource source)
        {
            source.Freeze();

            using var stream = new MemoryStream();

            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(
                BitmapFrame.Create(source));

            encoder.Save(stream);

            return new ThumbnailResult(
                stream.ToArray(),
                source.PixelWidth,
                source.PixelHeight);
        }

        public static BitmapSource ToBgra32(
            BitmapSource source)
        {
            var converted = new FormatConvertedBitmap(
                source,
                PixelFormats.Pbgra32,
                null,
                0);

            converted.Freeze();

            return converted;
        }
    }
}
