
using Microsoft.Data.Sqlite;
using System.Globalization;
using UnityCommander.Diagnostics.Abstraction;

namespace UnityCommander.Diagnostics.Storage.SQLite;

public sealed class SqliteDiagnosticsStorage : IDiagnosticsStorage
{
    private readonly SqliteConnectionFactory _connectionFactory;
    private readonly DatabaseInitializer _initializer;

    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    private bool _initialized;
    private bool _disposed;

    public SqliteDiagnosticsStorage(
        SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _initializer = new DatabaseInitializer();
    }

    public async ValueTask InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        await _initializationLock.WaitAsync(cancellationToken);

        try
        {
            if (_initialized)
            {
                return;
            }

            await using var connection =
                _connectionFactory.CreateConnection();

            await connection.OpenAsync(cancellationToken);

            await _initializer.InitializeAsync(
                connection,
                cancellationToken);

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    public async ValueTask CreateSessionAsync(
        DiagnosticSession session,
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText =
            """
            INSERT INTO Sessions
            (
                Id,
                Name,
                StartedAt,
                FinishedAt,
                ApplicationVersion,
                GitCommit,
                Configuration
            )
            VALUES
            (
                $id,
                $name,
                $startedAt,
                $finishedAt,
                $applicationVersion,
                $gitCommit,
                $configuration
            );
            """;

        command.Parameters.AddWithValue(
            "$id",
            session.Id.ToString("D"));

        command.Parameters.AddWithValue(
            "$name",
            session.Name);

        command.Parameters.AddWithValue(
            "$startedAt",
            FormatDate(session.StartedAt));

        command.Parameters.AddWithValue(
            "$finishedAt",
            session.FinishedAt.HasValue
                ? FormatDate(session.FinishedAt.Value)
                : DBNull.Value);

        command.Parameters.AddWithValue(
            "$applicationVersion",
            session.ApplicationVersion);

        command.Parameters.AddWithValue(
            "$gitCommit",
            (object?)session.GitCommit ?? DBNull.Value);

        command.Parameters.AddWithValue(
            "$configuration",
            (object?)session.Configuration ?? DBNull.Value);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async ValueTask WriteBatchAsync(
        IReadOnlyList<StoredDiagnosticRecord> records,
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        if (records.Count == 0)
        {
            return;
        }

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        using var transaction = connection.BeginTransaction();

        await using var command = connection.CreateCommand();

        command.Transaction = transaction;

        command.CommandText =
            """
            INSERT INTO DiagnosticRecords
            (
                Id,
                SessionId,
                Kind,
                Timestamp,
                Source,
                Name,
                PluginId,
                DurationTicks,
                PayloadJson
            )
            VALUES
            (
                $id,
                $sessionId,
                $kind,
                $timestamp,
                $source,
                $name,
                $pluginId,
                $durationTicks,
                $payloadJson
            );
            """;

        command.Parameters.Add("$id", SqliteType.Text);
        command.Parameters.Add("$sessionId", SqliteType.Text);
        command.Parameters.Add("$kind", SqliteType.Integer);
        command.Parameters.Add("$timestamp", SqliteType.Text);
        command.Parameters.Add("$source", SqliteType.Text);
        command.Parameters.Add("$name", SqliteType.Text);
        command.Parameters.Add("$pluginId", SqliteType.Text);
        command.Parameters.Add("$durationTicks", SqliteType.Integer);
        command.Parameters.Add("$payloadJson", SqliteType.Text);

        foreach (StoredDiagnosticRecord record in records)
        {
            command.Parameters["$id"].Value =
                record.Id.ToString("D");

            command.Parameters["$sessionId"].Value =
                record.SessionId.ToString("D");

            command.Parameters["$kind"].Value =
                (int)record.Kind;

            command.Parameters["$timestamp"].Value =
                FormatDate(record.Timestamp);

            command.Parameters["$source"].Value =
                record.Source;

            command.Parameters["$name"].Value =
                record.Name;

            command.Parameters["$pluginId"].Value =
                (object?)record.PluginId ?? DBNull.Value;

            command.Parameters["$durationTicks"].Value =
                record.DurationTicks.HasValue
                    ? record.DurationTicks.Value
                    : DBNull.Value;

            command.Parameters["$payloadJson"].Value =
                record.PayloadJson;

            await command.ExecuteNonQueryAsync(
                cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async ValueTask CompleteSessionAsync(
        Guid sessionId,
        DateTimeOffset completedAt,
        CancellationToken cancellationToken = default)
    {
        EnsureInitialized();

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText =
            """
            UPDATE Sessions
            SET FinishedAt = $finishedAt
            WHERE Id = $id;
            """;

        command.Parameters.AddWithValue(
            "$finishedAt",
            FormatDate(completedAt));

        command.Parameters.AddWithValue(
            "$id",
            sessionId.ToString("D"));

        int affectedRows = await command.ExecuteNonQueryAsync(
            cancellationToken);

        if (affectedRows == 0)
        {
            throw new InvalidOperationException(
                $"Diagnostic session '{sessionId}' was not found.");
        }
    }

    private static string FormatDate(DateTimeOffset value)
    {
        return value
            .ToUniversalTime()
            .ToString("O", CultureInfo.InvariantCulture);
    }

    private void EnsureInitialized()
    {
        ThrowIfDisposed();

        if (!_initialized)
        {
            throw new InvalidOperationException(
                "Diagnostics storage has not been initialized.");
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public ValueTask DisposeAsync()
    {
        _disposed = true;
        _initializationLock.Dispose();

        return ValueTask.CompletedTask;
    }
}