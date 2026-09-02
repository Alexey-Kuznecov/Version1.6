
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

        public Task<long> AddAsync(
             IndexedFile file,
             CancellationToken cancellationToken = default)
             => _writer.AddAsync(file, cancellationToken);

        public Task AddRangeAsync(
            IEnumerable<IndexedFile> files,
            CancellationToken cancellationToken = default)
            => _writer.AddRangeAsync(files, cancellationToken);

        public Task UpdateAsync(
            IndexedFile file,
            CancellationToken cancellationToken = default)
            => _writer.UpdateAsync(file, cancellationToken);

        public Task DeleteAsync(
            long id,
            CancellationToken cancellationToken = default)
            => _writer.DeleteAsync(id, cancellationToken);
    }
}
