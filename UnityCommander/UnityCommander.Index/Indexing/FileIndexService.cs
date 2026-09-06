
using System.IO;
using UnityCommander.Index.Abstractions;
using UnityCommander.Index.Models;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Index.Indexing
{
    public sealed class FileIndexService : IFileIndexService
    {
        private readonly IFileIndexReader _reader;
        private readonly IFileIndexWriter _writer;
       
        private readonly ILogger _logger;

        public bool IsAvailable { get; }

        public FileIndexService(
            IFileIndexReader reader,
            IFileIndexWriter writer, 
            LoggerCreator loggerCreator)
        {
            _logger = loggerCreator.For<FileIndexService>(LogScope.Runtime);
            _reader = reader;
            _writer = writer;

            _logger.Info($"FileIndexService initialized with reader hash code: \n {_reader.GetHashCode()}, writer hash code: {_writer.GetHashCode()}"  );
        }

        public IAsyncEnumerable<IndexedFile> EnumerateAsync(
            CancellationToken cancellationToken = default)
            => _reader.EnumerateAsync(cancellationToken);

        public IAsyncEnumerable<IndexedFile> EnumerateChildrenAsync(
            long? parentId,
            CancellationToken cancellationToken = default)
            => _reader.EnumerateChildrenAsync(
                parentId,
                cancellationToken);

        public Task<IndexedFile?> GetAsync(
            long id,
            CancellationToken cancellationToken = default)
            => _reader.GetAsync(id, cancellationToken);

        public Task UpdateAsync(
            IndexedFile file,
            CancellationToken cancellationToken = default)
            => _writer.UpdateAsync(file, cancellationToken);

        public Task DeleteAsync(
            long id,
            CancellationToken cancellationToken = default)
            => _writer.DeleteAsync(id, cancellationToken);

        public async Task<IndexAddResult> AddAsync(
            string path,
            CancellationToken cancellationToken = default)
        {
            var indexed = CreateIndexedFile(path);

            var id = await _writer.AddAsync(
                indexed,
                cancellationToken);

            indexed.Id = id;

            return new IndexAddResult(indexed);
        }

        public async Task<IndexOperationResult> AddRecursiveAsync(
           string path,
           CancellationToken cancellationToken)
        {
            if (!Directory.Exists(path))
            {
                _logger.Info(
                    $"Directory not found: {path}");

                await Task.CompletedTask;
            }

            var directory = new DirectoryInfo(path);

           return await AddDirectoryRecursiveAsync(
                directory,
                null,
                cancellationToken);
        }

        private async Task<IndexOperationResult> AddDirectoryRecursiveAsync(
          DirectoryInfo directory,
          long? parentId,
          CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = new IndexOperationResult();

            var directoryFile = CreateIndexedDirectory(directory);
            directoryFile.ParentId = parentId;

            var directoryId = await _writer.AddAsync(
                directoryFile,
                cancellationToken);

            _logger.Info(
                $"Indexed: {directory.FullName} (Id: {directoryId}, ParentId: {parentId})");

            result.Added++;
            result.RootId = directoryId;
            result.Items = new List<IndexedFile>();

            foreach (var file in directory.EnumerateFiles())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var indexedFile = CreateIndexedFile(file);
                indexedFile.ParentId = directoryId;

                result.Items.Add(indexedFile);
            }

            if (result.Items.Count > 0)
            {
                await _writer.AddRangeAsync(
                    result.Items,
                    cancellationToken);

                result.Added += result.Items.Count;

                foreach (var file in result.Items)
                {
                    _logger.Info(
                        $"Indexed: {file.Path} (ParentId: {directoryId})");
                }
            }

            foreach (var childDirectory in directory.EnumerateDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var childResult = await AddDirectoryRecursiveAsync(
                    childDirectory,
                    directoryId,
                    cancellationToken);

                result.Added += childResult.Added;
            }

            return result;
        }

        private IndexedFile CreateIndexedFile(string path)
        {
            IndexedFile indexed;

            if (File.Exists(path))
            {
                var info = new FileInfo(path);

                indexed = CreateIndexedFile(info);
            }
            else
            {
                var info = new DirectoryInfo(path);

                indexed = CreateIndexedDirectory(info);
            }

            return indexed;
        }

        private static IndexedFile CreateIndexedFile(FileInfo file)
        {
            return new IndexedFile
            {
                Path = file.FullName,
                Name = file.Name,
                Extension = file.Extension,
                IsDirectory = false,
                Size = file.Length,
                CreationTime = file.CreationTime,
                LastWriteTime = file.LastWriteTime,
                LastAccessTime = file.LastAccessTime,
                Attributes = file.Attributes
            };
        }

        private static IndexedFile CreateIndexedDirectory(DirectoryInfo directory)
        {
            return new IndexedFile
            {
                Path = directory.FullName,
                Name = directory.Name,
                Extension = string.Empty,
                IsDirectory = true,
                Size = 0,
                CreationTime = directory.CreationTime,
                LastWriteTime = directory.LastWriteTime,
                LastAccessTime = directory.LastAccessTime,
                Attributes = directory.Attributes
            };
        }
    }
}
