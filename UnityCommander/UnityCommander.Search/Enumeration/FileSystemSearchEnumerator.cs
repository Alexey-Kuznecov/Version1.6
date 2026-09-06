
using System.IO;
using System.Runtime.CompilerServices;
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Enumeration
{
    public sealed class FileSystemSearchEnumerator : ISearchEnumerator
    {
        public async IAsyncEnumerable<SearchItem> EnumerateAsync(
            SearchScope scope,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var path in scope.Paths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!Directory.Exists(path))
                    continue;

                await foreach (var item in EnumerateDirectoryAsync(
                    path,
                    cancellationToken))
                {
                    yield return item;
                }
            }
        }

        private static async IAsyncEnumerable<SearchItem> EnumerateDirectoryAsync(
            string path,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IEnumerable<string> entries;

            try
            {
               entries = Directory.EnumerateFileSystemEntries(path);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }
            catch (DirectoryNotFoundException)
            {
                yield break;
            }
            catch (IOException)
            {
                yield break;
            }

            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool isDirectory;

                try
                {
                    isDirectory = Directory.Exists(entry);
                }
                catch
                {
                    continue;
                }

                if (isDirectory)
                {
                    await foreach (var child in EnumerateDirectoryAsync(
                        entry,
                        cancellationToken))
                    {
                        yield return child;
                    }

                    continue;
                }

                var info = new FileInfo(entry);

                yield return new SearchItem(entry, Path.GetFileName(entry))
                {
                    IsDirectory = isDirectory,
                    CreationTime = File.GetCreationTime(entry),
                    LastWriteTime = File.GetLastAccessTime(entry),
                    Size = info.Length
                };
            }
        }
    }
}
