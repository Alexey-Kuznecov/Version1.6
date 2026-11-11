using Microsoft.Data.Sqlite;
using System.Globalization;
using UnityCommander.Diagnostics.Abstraction;

namespace UnityCommander.Diagnostics.Storage.SQLite;

public sealed class SqliteDiagnosticsReader : IDiagnosticsReader
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public SqliteDiagnosticsReader(
        SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async ValueTask<IReadOnlyList<DiagnosticSession>> GetSessionsAsync(
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        if (limit <= 0 || limit > 10000)
        {
            throw new ArgumentOutOfRangeException(nameof(limit));
        }

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        command.CommandText =
            """
            SELECT
                Id,
                Name,
                StartedAt,
                FinishedAt,
                ApplicationVersion,
                GitCommit,
                Configuration
            FROM Sessions
            ORDER BY StartedAt DESC
            LIMIT $limit;
            """;

        command.Parameters.AddWithValue("$limit", limit);

        var sessions = new List<DiagnosticSession>();

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            sessions.Add(new DiagnosticSession(
                Guid.Parse(reader.GetString(0)),
                reader.GetString(1),
                ParseDate(reader.GetString(2)),
                reader.IsDBNull(3)
                    ? null
                    : ParseDate(reader.GetString(3)),
                reader.GetString(4),
                ReadNullableString(reader, 5),
                ReadNullableString(reader, 6)));
        }

        return sessions;
    }

    public async ValueTask<IReadOnlyList<DiagnosticRecordEntry>> ReadRecordsAsync(
        DiagnosticRecordQuery query,
        CancellationToken cancellationToken = default)
    {
        ValidateQuery(query);

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        string where = BuildWhereClause(command, query);

        command.CommandText =
            $"""
            SELECT
                Id,
                SessionId,
                Kind,
                Timestamp,
                Source,
                Name,
                PluginId,
                DurationTicks,
                PayloadJson
            FROM DiagnosticRecords
            WHERE {where}
            ORDER BY Timestamp DESC, Id DESC
            LIMIT $limit OFFSET $offset;
            """;

        command.Parameters.AddWithValue("$limit", query.Limit);
        command.Parameters.AddWithValue("$offset", query.Offset);

        var records = new List<DiagnosticRecordEntry>();

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            records.Add(new DiagnosticRecordEntry(
                Guid.Parse(reader.GetString(0)),
                Guid.Parse(reader.GetString(1)),
                (DiagnosticRecordKind)reader.GetInt32(2),
                ParseDate(reader.GetString(3)),
                reader.GetString(4),
                reader.GetString(5),
                ReadNullableString(reader, 6),
                reader.IsDBNull(7)
                    ? null
                    : reader.GetInt64(7),
                reader.GetString(8)));
        }

        return records;
    }

    public async ValueTask<long> CountRecordsAsync(
        DiagnosticRecordQuery query,
        CancellationToken cancellationToken = default)
    {
        ValidateQuery(query);

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();

        string where = BuildWhereClause(command, query);

        command.CommandText =
            $"""
            SELECT COUNT(*)
            FROM DiagnosticRecords
            WHERE {where};
            """;

        object? result = await command.ExecuteScalarAsync(
            cancellationToken);

        return Convert.ToInt64(
            result,
            CultureInfo.InvariantCulture);
    }

    private static string BuildWhereClause(
        SqliteCommand command,
        DiagnosticRecordQuery query)
    {
        var conditions = new List<string>
        {
            "SessionId = $sessionId"
        };

        command.Parameters.AddWithValue(
            "$sessionId",
            query.SessionId.ToString("D"));

        if (query.Kind.HasValue)
        {
            conditions.Add("Kind = $kind");

            command.Parameters.AddWithValue(
                "$kind",
                (int)query.Kind.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Source))
        {
            conditions.Add("Source = $source");

            command.Parameters.AddWithValue(
                "$source",
                query.Source);
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            conditions.Add("Name LIKE $name");

            command.Parameters.AddWithValue(
                "$name",
                $"%{EscapeLike(query.Name)}%");
        }

        if (!string.IsNullOrWhiteSpace(query.PluginId))
        {
            conditions.Add("PluginId = $pluginId");

            command.Parameters.AddWithValue(
                "$pluginId",
                query.PluginId);
        }

        if (query.From.HasValue)
        {
            conditions.Add("Timestamp >= $from");

            command.Parameters.AddWithValue(
                "$from",
                FormatDate(query.From.Value));
        }

        if (query.To.HasValue)
        {
            conditions.Add("Timestamp <= $to");

            command.Parameters.AddWithValue(
                "$to",
                FormatDate(query.To.Value));
        }

        return string.Join(" AND ", conditions);
    }

    private static void ValidateQuery(
        DiagnosticRecordQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.SessionId == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid session ID is required.",
                nameof(query));
        }

        if (query.Limit <= 0 || query.Limit > 10000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query.Limit));
        }

        if (query.Offset < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(query.Offset));
        }

        if (query.From.HasValue &&
            query.To.HasValue &&
            query.From > query.To)
        {
            throw new ArgumentException(
                "The start date cannot be later than the end date.",
                nameof(query));
        }
    }

    private static DateTimeOffset ParseDate(string value)
    {
        return DateTimeOffset.Parse(
            value,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind);
    }

    private static string FormatDate(DateTimeOffset value)
    {
        return value
            .ToUniversalTime()
            .ToString("O", CultureInfo.InvariantCulture);
    }

    private static string? ReadNullableString(
        SqliteDataReader reader,
        int ordinal)
    {
        return reader.IsDBNull(ordinal)
            ? null
            : reader.GetString(ordinal);
    }

    private static string EscapeLike(string value)
    {
        return value
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }
}