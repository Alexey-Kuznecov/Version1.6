using Microsoft.Data.Sqlite;

namespace UnityCommander.Diagnostics.Storage.SQLite;

public sealed class SqliteConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(
        string databasePath,
        SqliteOpenMode mode = SqliteOpenMode.ReadWriteCreate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        string fullPath = Path.GetFullPath(databasePath);

        if (mode == SqliteOpenMode.ReadOnly)
        {
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    "Diagnostics database was not found.",
                    fullPath);
            }
        }
        else
        {
            string? directory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = fullPath,
            Mode = mode,
            Cache = SqliteCacheMode.Private,
            Pooling = true,
            ForeignKeys = true
        };

        _connectionString = builder.ToString();
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}