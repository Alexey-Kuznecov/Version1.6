
using System.Runtime.CompilerServices;
using UnityCommander.Copying.Core;

namespace UnityCommander.Copying.Strategies
{
    public class RecursiveFullDiscoveryStrategy : IFileDiscoveryStrategy
    {
        public async IAsyncEnumerable<DiscoveredItem> DiscoverAsync(
            string sourceRoot,
            string destinationRoot,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(sourceRoot))
                yield break;

            var stack = new Stack<string>();
            stack.Push(sourceRoot);

            while (stack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentSourcePath = stack.Pop();
                string relativePath = Path.GetRelativePath(sourceRoot, currentSourcePath);
                string destPath = Path.Combine(destinationRoot, relativePath);

                bool hasFilesInside = false;
                try
                {
                    hasFilesInside = Directory.EnumerateFiles(currentSourcePath).Any();
                }
                catch { /* нет доступа — считаем, что нет файлов */ }

                yield return new DiscoveredItem
                {
                    Source = currentSourcePath,
                    Destination = destPath,
                    Type = DiscoveredItemType.Directory,
                    HasFilesInside = hasFilesInside
                };

                // поддиректории
                IEnumerable<string> subdirs = Enumerable.Empty<string>();
                try { subdirs = Directory.EnumerateDirectories(currentSourcePath); } catch { /* skip */ }
                foreach (var dir in subdirs)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    stack.Push(dir);
                }

                // файлы
                IEnumerable<string> files = Enumerable.Empty<string>();
                try { files = Directory.EnumerateFiles(currentSourcePath); } catch { /* skip */ }
                foreach (var file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    FileInfo fi;
                    try { fi = new FileInfo(file); }
                    catch { continue; }

                    var rel = Path.GetRelativePath(sourceRoot, file);
                    var destFilePath = Path.Combine(destinationRoot, rel);

                    yield return new DiscoveredItem
                    {
                        Source = file,
                        Destination = destFilePath,
                        FileSize = fi.Length,
                        FileInfo = fi,
                        Type = DiscoveredItemType.File,
                        HasFilesInside = false
                    };

                    // экономим CPU/даём шанс переключиться на другие задачи
                    await Task.Yield();
                }
            }
        }
    }
}
