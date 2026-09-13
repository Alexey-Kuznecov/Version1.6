
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using UnityCommander.Search.Models;

namespace UnityCommander.Search.Enumeration
{
    public sealed class FileSystemSearchEnumerator : ISearchEnumerator
    {
        public async IAsyncEnumerable<SearchItem> EnumerateAsync(
            SearchScope scope,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var channel = Channel.CreateBounded<SearchItem>(
                new BoundedChannelOptions(256)
                {
                    SingleWriter = true,
                    SingleReader = true,
                    FullMode = BoundedChannelFullMode.Wait
                });

            _ = Task.Run(
                async () =>
                {
                    try
                    {
                        foreach (var path in scope.Paths)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            if (!Directory.Exists(path))
                                continue;

                            EnumerateDirectory(
                                path,
                                channel.Writer,
                                cancellationToken);
                        }

                        channel.Writer.TryComplete();
                    }
                    catch (OperationCanceledException)
                    {
                        channel.Writer.TryComplete();
                    }
                    catch (Exception ex)
                    {
                        channel.Writer.TryComplete(ex);
                    }

                    await Task.CompletedTask;
                },
                CancellationToken.None);

            await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
                yield return item;
        }

        private static void EnumerateDirectory(
            string path,
            ChannelWriter<SearchItem> writer,
            CancellationToken cancellationToken)
        {
            IEnumerable<string> entries;

            try
            {
                entries = Directory.EnumerateFileSystemEntries(path);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
            catch (IOException)
            {
                return;
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
                    EnumerateDirectory(
                        entry,
                        writer,
                        cancellationToken);

                    continue;
                }

                try
                {
                    var info = new FileInfo(entry);

                    var item = new SearchItem(
                        entry,
                        Path.GetFileName(entry))
                    {
                        IsDirectory = false,
                        CreationTime = File.GetCreationTime(entry),
                        LastWriteTime = File.GetLastAccessTime(entry),
                        Size = info.Length
                    };

                    writer.WriteAsync(
                        item,
                        cancellationToken)
                        .AsTask()
                        .GetAwaiter()
                        .GetResult();
                }
                catch (
                    UnauthorizedAccessException)
                {
                }
                catch (
                    IOException)
                {
                }
            }
        }
    }
}
