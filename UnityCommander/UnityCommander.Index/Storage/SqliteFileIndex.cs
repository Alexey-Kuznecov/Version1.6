
using Microsoft.Data.Sqlite;
using System.IO;
using UnityCommander.Index.Abstractions;
using UnityCommander.Index.Models;

namespace UnityCommander.Index.Storage
{
    public sealed class SqliteFileIndex :
        IFileIndexReader,
        IFileIndexWriter
    {
        private readonly string _connectionString;

        public SqliteFileIndex(string databasePath)
        {
            _connectionString =
                new SqliteConnectionStringBuilder
                {
                    DataSource = databasePath
                }.ToString();

            InitializeDatabase();
        }

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        private void InitializeDatabase()
        {
            using var connection = CreateConnection();
            connection.Open();

            using var command = connection.CreateCommand();

            command.CommandText =
                """
            CREATE TABLE IF NOT EXISTS Files
            (
                Id INTEGER PRIMARY KEY,
                ParentId INTEGER,
                Path TEXT NOT NULL,
                Name TEXT NOT NULL,
                Extension TEXT NOT NULL,
                IsDirectory INTEGER NOT NULL,
                Size INTEGER NOT NULL,
                CreationTime TEXT NOT NULL,
                LastWriteTime TEXT NOT NULL,
                LastAccessTime TEXT NOT NULL,
                Attributes INTEGER NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_Files_ParentId
                ON Files(ParentId);

            CREATE INDEX IF NOT EXISTS IX_Files_Name
                ON Files(Name);

            CREATE INDEX IF NOT EXISTS IX_Files_Extension
                ON Files(Extension);
            """;

            command.ExecuteNonQuery();
        }

        public async Task<long> AddAsync(
            IndexedFile file,
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText =
                """
            INSERT INTO Files
            (
                ParentId,
                Path,
                Name,
                Extension,
                IsDirectory,
                Size,
                CreationTime,
                LastWriteTime,
                LastAccessTime,
                Attributes
            )
            VALUES
            (
                $parentId,
                $path,
                $name,
                $extension,
                $isDirectory,
                $size,
                $creationTime,
                $lastWriteTime,
                $lastAccessTime,
                $attributes
            );
            """;

            AddInsertParameters(command, file);

            await command.ExecuteNonQueryAsync(cancellationToken);

            await using var idCommand = connection.CreateCommand();

            idCommand.CommandText =
                "SELECT last_insert_rowid();";

            var id = (long)(await idCommand.ExecuteScalarAsync(
                cancellationToken))!;

            return id;
        }

        private static void AddInsertParameters(
            SqliteCommand command,
            IndexedFile file)
        {
            AddParameters(command, file);
        }

        private static void AddUpdateParameters(
         SqliteCommand command,
         IndexedFile file)
        {
            command.Parameters.AddWithValue("$id", file.Id);
            AddParameters(command, file);
        }

        private static void AddParameters(
            SqliteCommand command,
            IndexedFile file)
        {
            command.Parameters.AddWithValue("$parentId", (object?)file.ParentId ?? DBNull.Value);
            command.Parameters.AddWithValue("$path", file.Path);
            command.Parameters.AddWithValue("$name", file.Name);
            command.Parameters.AddWithValue("$extension", file.Extension);
            command.Parameters.AddWithValue("$isDirectory", file.IsDirectory ? 1 : 0);
            command.Parameters.AddWithValue("$size", file.Size);
            command.Parameters.AddWithValue("$creationTime", file.CreationTime.Ticks);
            command.Parameters.AddWithValue("$lastWriteTime", file.LastWriteTime.Ticks);
            command.Parameters.AddWithValue("$lastAccessTime", file.LastAccessTime.Ticks);
            command.Parameters.AddWithValue("$attributes", (int)file.Attributes);
        }

        public async Task AddRangeAsync(
            IEnumerable<IndexedFile> files,
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var transaction =
                await connection.BeginTransactionAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.Transaction = (SqliteTransaction)transaction;

            command.CommandText =
                """
            INSERT INTO Files
            (
                ParentId,
                Path,
                Name,
                Extension,
                IsDirectory,
                Size,
                CreationTime,
                LastWriteTime,
                LastAccessTime,
                Attributes
            )
            VALUES
            (
                $parentId,
                $path,
                $name,
                $extension,
                $isDirectory,
                $size,
                $creationTime,
                $lastWriteTime,
                $lastAccessTime,
                $attributes
            );
            """;

            var id = command.Parameters.Add("$id", SqliteType.Integer);
            var parentId = command.Parameters.Add("$parentId", SqliteType.Integer);
            var path = command.Parameters.Add("$path", SqliteType.Text);
            var name = command.Parameters.Add("$name", SqliteType.Text);
            var extension = command.Parameters.Add("$extension", SqliteType.Text);
            var isDirectory = command.Parameters.Add("$isDirectory", SqliteType.Integer);
            var size = command.Parameters.Add("$size", SqliteType.Integer);
            var creationTime = command.Parameters.Add("$creationTime", SqliteType.Integer);
            var lastWriteTime = command.Parameters.Add("$lastWriteTime", SqliteType.Integer);
            var lastAccessTime = command.Parameters.Add("$lastAccessTime", SqliteType.Integer);
            var attributes = command.Parameters.Add("$attributes", SqliteType.Integer);

            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                parentId.Value =
                    (object?)file.ParentId ?? DBNull.Value;

                path.Value = file.Path;
                name.Value = file.Name;
                extension.Value = file.Extension;
                isDirectory.Value = file.IsDirectory ? 1 : 0;
                size.Value = file.Size;
                creationTime.Value = file.CreationTime.Ticks;
                lastWriteTime.Value = file.LastWriteTime.Ticks;
                lastAccessTime.Value = file.LastAccessTime.Ticks;
                attributes.Value = (int)file.Attributes;

                await command.ExecuteNonQueryAsync(
                    cancellationToken);
            }

            await transaction.CommitAsync(
                cancellationToken);
        }

        public async Task<IndexedFile?> GetAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText =
                """
            SELECT
                Id,
                ParentId,
                Path,
                Name,
                Extension,
                IsDirectory,
                Size,
                CreationTime,
                LastWriteTime,
                LastAccessTime,
                Attributes
            FROM Files
            WHERE Id = $id;
            """;

            command.Parameters.AddWithValue("$id", id);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
                return null;

            return ReadFile(reader);
        }

        private static IndexedFile ReadFile(
            SqliteDataReader reader)
        {
            return new IndexedFile
            {
                Id = reader.GetInt64(0),
                ParentId = reader.IsDBNull(1)
                    ? null
                    : reader.GetInt64(1),

                Path = reader.GetString(2),
                Name = reader.GetString(3),
                Extension = reader.GetString(4),

                IsDirectory = reader.GetInt64(5) != 0,

                Size = reader.GetInt64(6),

                CreationTime = new DateTime(
                    reader.GetInt64(7)),

                LastWriteTime = new DateTime(
                    reader.GetInt64(8)),

                LastAccessTime = new DateTime(
                    reader.GetInt64(9)),

                Attributes = (FileAttributes)reader.GetInt64(10)
            };
        }

        public async IAsyncEnumerable<IndexedFile> EnumerateAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText =
                """
            SELECT
                Id,
                ParentId,
                Path,
                Name,
                Extension,
                IsDirectory,
                Size,
                CreationTime,
                LastWriteTime,
                LastAccessTime,
                Attributes
            FROM Files;
            """;

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return ReadFile(reader);
            }
        }

        public async IAsyncEnumerable<IndexedFile> EnumerateChildrenAsync(
            long? parentId,
            [System.Runtime.CompilerServices.EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            if (parentId.HasValue)
            {
                command.CommandText =
                    """
        SELECT
            Id,
            ParentId,
            Path,
            Name,
            Extension,
            IsDirectory,
            Size,
            CreationTime,
            LastWriteTime,
            LastAccessTime,
            Attributes
        FROM Files
        WHERE ParentId = $parentId
        ORDER BY IsDirectory DESC, Name COLLATE NOCASE;
        """;

                command.Parameters.AddWithValue(
                    "$parentId",
                    parentId.Value);
            }
            else
            {
                command.CommandText =
                    """
        SELECT
            Id,
            ParentId,
            Path,
            Name,
            Extension,
            IsDirectory,
            Size,
            CreationTime,
            LastWriteTime,
            LastAccessTime,
            Attributes
        FROM Files
        WHERE ParentId IS NULL
        ORDER BY IsDirectory DESC, Name COLLATE NOCASE;
        """;
            }

            command.Parameters.AddWithValue(
                "$parentId",
                (object?)parentId ?? DBNull.Value);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                yield return ReadFile(reader);
            }
        }

        public async Task UpdateAsync(
            IndexedFile file,
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText =
                """
            UPDATE Files
            SET
                ParentId = $parentId,
                Path = $path,
                Name = $name,
                Extension = $extension,
                IsDirectory = $isDirectory,
                Size = $size,
                CreationTime = $creationTime,
                LastWriteTime = $lastWriteTime,
                LastAccessTime = $lastAccessTime,
                Attributes = $attributes
            WHERE Id = $id;
            """;

            AddUpdateParameters(command, file);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task DeleteAsync(
            long id,
            CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText =
                """
                DELETE FROM Files
                WHERE Id = $id;
                """;

            command.Parameters.AddWithValue("$id", id);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
