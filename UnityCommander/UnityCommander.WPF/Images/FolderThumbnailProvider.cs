
using System.IO;
using System.Text;
using UnityCommander.Abstractions.Thumbnail;

namespace UnityCommander.WPF.Images
{
    public sealed class FolderThumbnailProvider : IThumbnailProvider
    {
        private static readonly HashSet<string> PreviewExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
            ".jpg",
            ".jpeg",
            ".png",
            ".bmp",
            ".gif",
            ".webp",
            ".tif",
            ".tiff",
            ".avif",

            ".mp4",
            ".mkv",
            ".mov",
            ".avi",
            ".webm",
            ".wmv",
            ".m4v"
            };

        private readonly IShellThumbnailExtractor _shell;

        public FolderThumbnailProvider(
            IShellThumbnailExtractor shell)
        {
            _shell = shell;
        }

        public string Id => "unity.folder";

        public int Priority => 100;

        public bool CanProvide(string path)
        {
            return Directory.Exists(path);
        }

        public async ValueTask<string?> GetCacheKeyAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token)
        {
            var candidates = await GetCandidatesAsync(
                path,
                token);

            if (candidates.Count == 0)
                return null;

            var builder = new StringBuilder();

            builder.Append("folder|");
            builder.Append(Path.GetFullPath(path));

            foreach (var file in candidates)
            {
                try
                {
                    var info = new FileInfo(file);

                    builder.Append('|');
                    builder.Append(file);
                    builder.Append('|');
                    builder.Append(info.Length);
                    builder.Append('|');
                    builder.Append(info.LastWriteTimeUtc.Ticks);
                }
                catch
                {
                }
            }

            return builder.ToString();
        }

        public async Task<ThumbnailResult?> GetAsync(
            string path,
            ThumbnailRequest request,
            CancellationToken token)
        {
            var candidates = await GetCandidatesAsync(
                path,
                token);

            if (candidates.Count == 0)
                return null;

            var tasks = candidates.Select(
                 file => _shell.ExtractAsync(
                     file,
                     request.Size,
                     token));

            var results = await Task.WhenAll(tasks);

            var thumbnails = results
                .Where(x => x is not null)
                .Cast<ThumbnailResult>()
                .Take(4)
                .ToList();

            if (thumbnails.Count == 0)
                return null;

            return FolderThumbnailComposer.Compose(
                thumbnails,
                request.Size);
        }

        private static Task<IReadOnlyList<string>> GetCandidatesAsync(
            string path,
            CancellationToken token)
        {
            return Task.Run(
                () =>
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();

                        var result = Directory
                            .EnumerateFiles(
                                path,
                                "*",
                                SearchOption.TopDirectoryOnly)
                            .Where(IsCandidate)
                            .OrderBy(
                                x => Path.GetFileName(x),
                                StringComparer.OrdinalIgnoreCase)
                            .Take(8)
                            .ToList();

                        return (IReadOnlyList<string>)result;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return Array.Empty<string>();
                    }
                    catch (DirectoryNotFoundException)
                    {
                        return Array.Empty<string>();
                    }
                    catch (IOException)
                    {
                        return Array.Empty<string>();
                    }
                },
                token);
        }

        private static bool IsCandidate(string path)
        {
            return PreviewExtensions.Contains(
                Path.GetExtension(path));
        }
    }
}
