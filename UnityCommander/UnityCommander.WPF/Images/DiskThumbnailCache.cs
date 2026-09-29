
using System.Buffers.Binary;
using System.IO;
using System.Security.Cryptography;
using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.Common;

namespace UnityCommander.WPF.Images
{
    public sealed class DiskThumbnailCache : IThumbnailCache
    {
        private static readonly byte[] Magic =
        [
            (byte)'U',
            (byte)'C',
            (byte)'T',
            (byte)'H'
        ];

        private const int Version = 1;

        private readonly string _root;

        public int Priority => 0;

        public DiskThumbnailCache(UnityCommanderPath path)
        {
            if (string.IsNullOrWhiteSpace(path.ThumbnailCache))
                throw new ArgumentException(
                    "Cache root is required.",
                    nameof(path.ThumbnailCache));

            _root = path.ThumbnailCache;

            Directory.CreateDirectory(_root);
        }

        public async Task<ThumbnailResult?> GetAsync(
            string key,
            CancellationToken token)
        {
            var path = GetFilePath(key);

            if (!File.Exists(path))
                return null;

            try
            {
                var data = await File.ReadAllBytesAsync(
                    path,
                    token);

                if (data.Length < 16)
                    return null;

                if (!data.AsSpan(0, 4).SequenceEqual(Magic))
                    return null;

                var version =
                    BinaryPrimitives.ReadInt32LittleEndian(
                        data.AsSpan(4, 4));

                if (version != Version)
                    return null;

                var width =
                    BinaryPrimitives.ReadInt32LittleEndian(
                        data.AsSpan(8, 4));

                var height =
                    BinaryPrimitives.ReadInt32LittleEndian(
                        data.AsSpan(12, 4));

                if (width <= 0 || height <= 0)
                    return null;

                var png = data[16..];

                return png.Length == 0
                    ? null
                    : new ThumbnailResult(
                        png,
                        width,
                        height);
            }
            catch
            {
                TryDelete(path);
                return null;
            }
        }

        public async Task SetAsync(
            string key,
            ThumbnailResult value,
            CancellationToken token)
        {
            var path = GetFilePath(key);
            var directory = Path.GetDirectoryName(path)!;

            Directory.CreateDirectory(directory);

            var png = value.PngData;

            var data = new byte[16 + png.Length];

            Magic.CopyTo(data, 0);

            BinaryPrimitives.WriteInt32LittleEndian(
                data.AsSpan(4, 4),
                Version);

            BinaryPrimitives.WriteInt32LittleEndian(
                data.AsSpan(8, 4),
                value.Width);

            BinaryPrimitives.WriteInt32LittleEndian(
                data.AsSpan(12, 4),
                value.Height);

            png.CopyTo(data.AsSpan(16));

            var tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                await File.WriteAllBytesAsync(
                    tempPath,
                    data,
                    token);

                File.Move(
                    tempPath,
                    path,
                    overwrite: true);
            }
            finally
            {
                TryDelete(tempPath);
            }
        }

        private string GetFilePath(string key)
        {
            var hash = Convert.ToHexString(
                SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(key)));

            var shard = hash[..2];

            return Path.Combine(
                _root,
                shard,
                hash + ".ucth");
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch
            {
            }
        }
    }
}
