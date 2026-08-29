
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
            foreach (var entry in Directory.EnumerateFileSystemEntries(path))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (File.Exists(entry))
                {
                    yield return new SearchItem(entry);
                }

                if (Directory.Exists(entry))
                {
                    await foreach (var item in EnumerateDirectoryAsync(
                        entry,
                        cancellationToken))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
