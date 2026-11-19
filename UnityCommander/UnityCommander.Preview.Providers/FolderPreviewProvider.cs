using UnityCommander.Abstractions.Thumbnail;
using UnityCommander.Preview.Abstraction;
using UnityCommander.Preview.Abstraction.Models;

namespace UnityCommander.Preview.Providers
{
    public sealed class FolderPreviewProvider : IPreviewProvider
    {
        private static readonly HashSet<string> PreviewExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif",
            ".webp", ".tif", ".tiff", ".avif",
            ".mp4", ".mkv", ".mov", ".avi", ".webm",
            ".wmv", ".m4v"
        };

        public string Id => "core.preview.folders";

        public int Priority => 100;

        private readonly IThumbnailService _thumbnailService;

        public FolderPreviewProvider(IThumbnailService thumbnailService)
        {
            _thumbnailService = thumbnailService;
        }

        public ValueTask<bool> CanPreviewAsync(
            PreviewRequest request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return ValueTask.FromResult(
                Directory.Exists(request.Path));
        }

        public async Task<PreviewResult> CreatePreviewAsync(
            PreviewRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var content = await Task.Run(
                () => CreateContent(request.Path, cancellationToken),
                cancellationToken).ConfigureAwait(false);

            var thumbnails = await GetThumbnailsAsync(
                request.Path,
                cancellationToken).ConfigureAwait(false);

            var result = new FolderPreviewContent(
                content.Path,
                content.FileCount,
                content.DirectoryCount,
                content.TotalSize,
                content.CreationTime,
                content.LastModified,
                thumbnails);

            return new PreviewResult(result);
        }

        private static (
            string Path,
            int FileCount,
            int DirectoryCount,
            long TotalSize,
            DateTime CreationTime,
            DateTime LastModified) CreateContent(
                string path,
                CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var root = new DirectoryInfo(path);
            root.Refresh();

            if (!root.Exists)
                throw new DirectoryNotFoundException(path);

            var creationTime = root.CreationTime;
            var lastModified = root.LastWriteTime;

            var fileCount = 0;
            var directoryCount = 0;
            long totalSize = 0;

            var directories = new Stack<DirectoryInfo>();
            directories.Push(root);

            while (directories.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var current = directories.Pop();
                var isRoot = ReferenceEquals(current, root);

                foreach (var file in GetFilesSafe(current))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        var length = file.Length;

                        totalSize = checked(totalSize + length);

                        if (isRoot)
                            fileCount++;
                    }
                    catch (IOException)
                    {
                        // Файл мог исчезнуть или стать недоступным.
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Нет доступа к файлу.
                    }
                }

                foreach (var directory in GetDirectoriesSafe(current))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (isRoot)
                        directoryCount++;

                    try
                    {
                        if ((directory.Attributes & FileAttributes.ReparsePoint) != 0)
                            continue;

                        directories.Push(directory);
                    }
                    catch (IOException)
                    {
                        // Каталог недоступен.
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Нет доступа к каталогу.
                    }
                }
            }

            return (
                root.FullName,
                fileCount,
                directoryCount,
                totalSize,
                creationTime,
                lastModified);
        }

        private static IEnumerable<FileInfo> GetFilesSafe(
            DirectoryInfo directory)
        {
            try
            {
                return directory.GetFiles();
            }
            catch (IOException)
            {
                return Array.Empty<FileInfo>();
            }
            catch (UnauthorizedAccessException)
            {
                return Array.Empty<FileInfo>();
            }
        }

        private static IEnumerable<DirectoryInfo> GetDirectoriesSafe(
            DirectoryInfo directory)
        {
            try
            {
                return directory.GetDirectories();
            }
            catch (IOException)
            {
                return Array.Empty<DirectoryInfo>();
            }
            catch (UnauthorizedAccessException)
            {
                return Array.Empty<DirectoryInfo>();
            }
        }

        private async Task<IReadOnlyList<ThumbnailResult>> GetThumbnailsAsync(
            string folderPath,
            CancellationToken token)
        {
            var files = await Task.Run(
                () => Directory
                    .EnumerateFiles(folderPath, "*", SearchOption.TopDirectoryOnly)
                    .Where(IsCandidate)
                    .Take(8)
                    .OrderBy(
                        x => Path.GetFileName(x),
                        StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                token);

            var tasks = files.Select(file =>
                _thumbnailService.GetAsync(
                    file,
                    new ThumbnailRequest(128),
                    token));

            var results = await Task.WhenAll(tasks);

            return results
                .Where(x => x is not null)
                .Cast<ThumbnailResult>()
                .Take(4)
                .ToArray();
        }

        private static bool IsCandidate(string path)
        {
            return PreviewExtensions.Contains(Path.GetExtension(path));
        }
    }
}
