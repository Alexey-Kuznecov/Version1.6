
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace UnityCommander.Diagnostics.Storage.SQLite;

internal sealed class DatabaseInitializer
{
    private const int CurrentSchemaVersion = 1;

    public async Task InitializeAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken = default)
    {
        await EnableWalAsync(
            connection,
            cancellationToken);

        int version = await GetSchemaVersionAsync(
            connection,
            cancellationToken);

        if (version > CurrentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Database schema version {version} is newer " +
                $"than supported version {CurrentSchemaVersion}.");
        }

        if (version < 1)
        {
            await CreateInitialSchemaAsync(
                connection,
                cancellationToken);
        }
    }

    private static async Task EnableWalAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA journal_mode = WAL;";

            await command.ExecuteScalarAsync(
                cancellationToken);
        }

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA synchronous = NORMAL;";

            await command.ExecuteNonQueryAsync(
                cancellationToken);
        }
    }

    private static async Task<int> GetSchemaVersionAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();

        command.CommandText = "PRAGMA user_version;";

        object? result = await command.ExecuteScalarAsync(
            cancellationToken);

        return Convert.ToInt32(
            result,
            CultureInfo.InvariantCulture);
    }

    private static async Task CreateInitialSchemaAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        using var transaction = connection.BeginTransaction();

        string[] commands =
        {
            """
            CREATE TABLE IF NOT EXISTS Sessions
            (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                StartedAt TEXT NOT NULL,
                FinishedAt TEXT NULL,
                ApplicationVersion TEXT NOT NULL,
                GitCommit TEXT NULL,
                Configuration TEXT NULL
            );
            """,

            """
            CREATE TABLE IF NOT EXISTS DiagnosticRecords
            (
                Id TEXT PRIMARY KEY,
                SessionId TEXT NOT NULL,
                Kind INTEGER NOT NULL,
                Timestamp TEXT NOT NULL,
                Source TEXT NOT NULL,
                Name TEXT NOT NULL,
                PluginId TEXT NULL,
                DurationTicks INTEGER NULL,
                PayloadJson TEXT NOT NULL,

                FOREIGN KEY (SessionId)
                    REFERENCES Sessions(Id)
                    ON DELETE CASCADE
            );
            """,

            """
            CREATE INDEX IF NOT EXISTS IX_Records_Session_Timestamp
                ON DiagnosticRecords(SessionId, Timestamp);
            """,

            """
            CREATE INDEX IF NOT EXISTS IX_Records_Session_Kind
                ON DiagnosticRecords(SessionId, Kind);
            """,

            """
            CREATE INDEX IF NOT EXISTS IX_Records_Name
                ON DiagnosticRecords(Name);
            """,

            """
            CREATE INDEX IF NOT EXISTS IX_Records_Plugin
                ON DiagnosticRecords(PluginId);
            """
        };

        foreach (string sql in commands)
        {
            await using var command = connection.CreateCommand();

            command.Transaction = transaction;
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync(
                cancellationToken);
        }

        await using (var versionCommand = connection.CreateCommand())
        {
            versionCommand.Transaction = transaction;
            versionCommand.CommandText = "PRAGMA user_version = 1;";

            await versionCommand.ExecuteNonQueryAsync(
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }
}