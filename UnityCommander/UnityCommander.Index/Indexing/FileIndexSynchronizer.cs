
using System.IO;
using UnityCommander.Index.Abstractions;
using UnityCommander.Index.Models;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Index.Indexing
{
    public sealed class FileIndexSynchronizer : IFileIndexSynchronizer
    {
        private readonly IFileIndexChangeQueue _queue;
        private readonly IFileIndexService _indexService;
        private readonly ILogger _logger;

        private FileSystemWatcher? _watcher;

        public FileIndexSynchronizer(
            IFileIndexChangeQueue queue,
            IFileIndexService indexService,
            LoggerCreator loggerCreator)
        {
            _queue = queue;
            _indexService = indexService;
            _logger = loggerCreator.For<FileIndexSynchronizer>(LogScope.Runtime);
        }

        public Task StartAsync(
            string path,
            CancellationToken cancellationToken = default)
        {
            _watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = true,
                NotifyFilter =
                    NotifyFilters.FileName |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Size
            };

            _watcher.Created += OnCreated;
            _watcher.Changed += OnChanged;
            _watcher.Deleted += OnDeleted;

            _watcher.EnableRaisingEvents = true;

            return Task.CompletedTask;
        }

        public Task StopAsync(
            CancellationToken cancellationToken = default)
        {
            if (_watcher == null)
                return Task.CompletedTask;

            _watcher.EnableRaisingEvents = false;

            _watcher.Created -= OnCreated;
            _watcher.Changed -= OnChanged;
            _watcher.Deleted -= OnDeleted;
            _watcher.Renamed += OnRenamed;

            _watcher.Dispose();
            _watcher = null;

            return Task.CompletedTask;
        }

        private void OnCreated(
            object sender,
            FileSystemEventArgs e)
        {
            if (ShouldIgnore(e.FullPath)) return;

            _queue.Enqueue(
                new IndexChange(
                    e.FullPath,
                    IndexChangeType.Created, ""));

            _logger.Info(
                $"File created: {e.FullPath}");
        }

        private void OnChanged(
            object sender,
            FileSystemEventArgs e)
        {
            if (ShouldIgnore(e.FullPath)) return;

            _queue.Enqueue(
                new IndexChange(
                    e.FullPath,
                    IndexChangeType.Changed, ""));
            _logger.Info(
                $"File changed: {e.FullPath}");
        }

        private void OnDeleted(
            object sender,
            FileSystemEventArgs e)
        {
             _queue.Enqueue(
                new IndexChange(
                    e.FullPath,
                    IndexChangeType.Deleted, ""));
            _logger.Info(
                $"File deleted: {e.FullPath}");
        }

        private void OnRenamed(
            object sender,
            RenamedEventArgs e)
        {
            if (ShouldIgnore(e.OldFullPath) ||
                ShouldIgnore(e.FullPath))
                return;

            _queue.Enqueue(
                new IndexChange(
                    e.OldFullPath,
                    IndexChangeType.Renamed,
                    e.FullPath));

            _logger.Info(
                $"File renamed: {e.OldFullPath} -> {e.FullPath}");
        }

        private static bool ShouldIgnore(string path)
        {
            var parts = path.Split(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            return parts.Any(part =>
                part.Equals(
                    "$RECYCLE.BIN",
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}
