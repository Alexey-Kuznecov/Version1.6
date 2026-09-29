
using System.IO;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class ShellThumbnailProvider : IThumbnailProvider
    {
        private readonly IShellThumbnailExtractor _extractor;

        public ShellThumbnailProvider(
            IShellThumbnailExtractor extractor)
        {
            _extractor = extractor;
        }

        public string Id => "windows.shell";

        public int Priority => 0;

        public bool CanProvide(string path)
        {
            return File.Exists(path) ||
                   Directory.Exists(path);
        }

        public ValueTask<string?> GetCacheKeyAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            try
            {
                var fullPath = Path.GetFullPath(path);

                if (File.Exists(fullPath))
                {
                    var info = new FileInfo(fullPath);

                    return ValueTask.FromResult<string?>(
                        $"file|{fullPath}|{info.Length}|{info.LastWriteTimeUtc.Ticks}");
                }

                if (Directory.Exists(fullPath))
                {
                    var info = new DirectoryInfo(fullPath);

                    return ValueTask.FromResult<string?>(
                        $"dir|{fullPath}|{info.LastWriteTimeUtc.Ticks}");
                }
            }
            catch
            {
            }

            return ValueTask.FromResult<string?>(null);
        }

        public Task<ThumbnailResult?> GetAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token)
        {
            return _extractor.ExtractAsync(
                path,
                request.Size,
                token);
        }
    }
}
