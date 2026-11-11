
using System.Buffers.Binary;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.Common;
using UnityCommander.Diagnostics.Tracing;

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

        private readonly IDiagnosticTrace _trace;

        public int Priority => 0;

        public DiskThumbnailCache(
            UnityCommanderPath path,
            IDiagnosticTrace trace)
        {
            if (string.IsNullOrWhiteSpace(path.ThumbnailCache))
            {
                throw new ArgumentException(
                    "Cache root is required.",
                    nameof(path.ThumbnailCache));
            }

            _root = path.ThumbnailCache;
            _trace = trace;

            Directory.CreateDirectory(_root);
        }

        public async Task<ThumbnailResult?> GetAsync(
            string key,
            CancellationToken token)
        {
            using var trace = _trace.Begin(
                "thumbnail.cache.disk",
                "get",
                DiagnosticTraceData.Of(
                    ("key", key)));

            string path = string.Empty;

            try
            {
                using (var pathTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "resolve-path"))
                {
                    path = GetFilePath(key);

                    pathTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("path", path)));
                }

                bool exists;

                using (var existsTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "file-exists",
                    DiagnosticTraceData.Of(
                        ("path", path))))
                {
                    exists = File.Exists(path);

                    existsTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("exists", exists)));
                }

                if (!exists)
                {
                    trace.Complete(
                        DiagnosticTraceData.Of(
                            ("hit", false),
                            ("reason", "file-not-found")));

                    return null;
                }

                byte[] data;

                using (var readTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "read-file",
                    DiagnosticTraceData.Of(
                        ("path", path))))
                {
                    try
                    {
                        data = await File.ReadAllBytesAsync(
                            path,
                            token);

                        readTrace.Complete(
                            DiagnosticTraceData.Of(
                                ("bytes", data.Length)));
                    }
                    catch (Exception exception)
                    {
                        readTrace.Fail(exception);
                        throw;
                    }
                }

                using var validationTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "validate-header",
                    DiagnosticTraceData.Of(
                        ("fileBytes", data.Length)));

                if (data.Length < 16)
                {
                    validationTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("valid", false),
                            ("reason", "header-too-short")));

                    trace.Complete(
                        DiagnosticTraceData.Of(
                            ("hit", false),
                            ("reason", "header-too-short"),
                            ("fileBytes", data.Length)));

                    return null;
                }

                if (!data.AsSpan(0, 4).SequenceEqual(Magic))
                {
                    validationTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("valid", false),
                            ("reason", "invalid-magic")));

                    trace.Complete(
                        DiagnosticTraceData.Of(
                            ("hit", false),
                            ("reason", "invalid-magic")));

                    return null;
                }

                int version =
                    BinaryPrimitives.ReadInt32LittleEndian(
                        data.AsSpan(4, 4));

                if (version != Version)
                {
                    validationTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("valid", false),
                            ("reason", "unsupported-version"),
                            ("version", version)));

                    trace.Complete(
                        DiagnosticTraceData.Of(
                            ("hit", false),
                            ("reason", "unsupported-version"),
                            ("version", version)));

                    return null;
                }

                int width =
                    BinaryPrimitives.ReadInt32LittleEndian(
                        data.AsSpan(8, 4));

                int height =
                    BinaryPrimitives.ReadInt32LittleEndian(
                        data.AsSpan(12, 4));

                if (width <= 0 || height <= 0)
                {
                    validationTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("valid", false),
                            ("reason", "invalid-dimensions"),
                            ("width", width),
                            ("height", height)));

                    trace.Complete(
                        DiagnosticTraceData.Of(
                            ("hit", false),
                            ("reason", "invalid-dimensions")));

                    return null;
                }

                validationTrace.Complete(
                    DiagnosticTraceData.Of(
                        ("valid", true),
                        ("version", version),
                        ("width", width),
                        ("height", height)));

                ThumbnailResult result;

                using (var extractTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "extract-png",
                    DiagnosticTraceData.Of(
                        ("fileBytes", data.Length))))
                {
                    var png = data[16..];

                    if (png.Length == 0)
                    {
                        extractTrace.Complete(
                            DiagnosticTraceData.Of(
                                ("hasData", false)));

                        trace.Complete(
                            DiagnosticTraceData.Of(
                                ("hit", false),
                                ("reason", "empty-png")));

                        return null;
                    }

                    result = new ThumbnailResult(
                        png,
                        width,
                        height);

                    extractTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("pngBytes", png.Length),
                            ("width", width),
                            ("height", height)));
                }

                trace.Complete(
                    DiagnosticTraceData.Of(
                        ("hit", true),
                        ("width", result.Width),
                        ("height", result.Height),
                        ("pngBytes", result.PngData.Length),
                        ("fileBytes", data.Length)));

                return result;
            }
            catch (OperationCanceledException exception)
                when (token.IsCancellationRequested)
            {
                trace.Fail(
                    exception,
                    DiagnosticTraceData.Of(
                        ("cancelled", true),
                        ("fileDeleted", false)));

                throw;
            }
            catch (Exception exception)
            {
                bool deleted = !string.IsNullOrEmpty(path)
                    && TryDelete(path);

                trace.Fail(
                    exception,
                    DiagnosticTraceData.Of(
                        ("fileDeleted", deleted),
                        ("fallback", "cache-miss")));

                return null;
            }
        }

        public async Task SetAsync(
            string key,
            ThumbnailResult value,
            CancellationToken token)
        {
            using var trace = _trace.Begin(
                "thumbnail.cache.disk",
                "set",
                DiagnosticTraceData.Of(
                    ("key", key),
                    ("width", value.Width),
                    ("height", value.Height),
                    ("pngBytes", value.PngData.Length)));

            string? path = null;

            string? tempPath = null;

            try
            {
                using (var pathTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "resolve-path"))
                {
                    path = GetFilePath(key);

                    pathTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("path", path)));
                }

                string directory = Path.GetDirectoryName(path)!;

                using (var directoryTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "create-directory",
                    DiagnosticTraceData.Of(
                        ("directory", directory))))
                {
                    Directory.CreateDirectory(directory);

                    directoryTrace.Complete();
                }

                byte[] data;

                using (var prepareTrace = _trace.Begin(
                    "thumbnail.cache.disk",
                    "prepare-data"))
                {
                    var png = value.PngData;

                    data = new byte[16 + png.Length];

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

                    prepareTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("fileBytes", data.Length),
                            ("pngBytes", png.Length)));
                }

                tempPath = path + "." +
                           Guid.NewGuid().ToString("N") +
                           ".tmp";

                try
                {
                    using (var writeTrace = _trace.Begin(
                        "thumbnail.cache.disk",
                        "write-temp-file",
                        DiagnosticTraceData.Of(
                            ("path", tempPath),
                            ("bytes", data.Length))))
                    {
                        await File.WriteAllBytesAsync(
                            tempPath,
                            data,
                            token);

                        writeTrace.Complete();
                    }

                    using (var moveTrace = _trace.Begin(
                        "thumbnail.cache.disk",
                        "replace-cache-file",
                        DiagnosticTraceData.Of(
                            ("source", tempPath),
                            ("destination", path))))
                    {
                        File.Move(
                            tempPath,
                            path,
                            overwrite: true);

                        moveTrace.Complete();
                    }
                }
                finally
                {
                    using var cleanupTrace = _trace.Begin(
                        "thumbnail.cache.disk",
                        "cleanup-temp-file");

                    bool deleted = TryDelete(tempPath);

                    cleanupTrace.Complete(
                        DiagnosticTraceData.Of(
                            ("deleted", deleted)));
                }

                trace.Complete(
                    DiagnosticTraceData.Of(
                        ("success", true),
                        ("fileBytes", data.Length),
                        ("path", path)));
            }
            catch (Exception exception)
            {
                trace.Fail(
                    exception,
                    DiagnosticTraceData.Of(
                        ("path", path),
                        ("tempPath", tempPath)));

                throw;
            }
        }

        private string GetFilePath(string key)
        {
            var hash = Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(key)));

            var shard = hash[..2];

            return Path.Combine(
                _root,
                shard,
                hash + ".ucth");
        }

        private static bool TryDelete(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return false;

                File.Delete(path);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
