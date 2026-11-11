namespace UnityCommander.Diagnostics.Abstraction;

public interface IDiagnosticsReader
{
    ValueTask<IReadOnlyList<DiagnosticSession>> GetSessionsAsync(
        int limit = 100,
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyList<DiagnosticRecordEntry>> ReadRecordsAsync(
        DiagnosticRecordQuery query,
        CancellationToken cancellationToken = default);

    ValueTask<long> CountRecordsAsync(
        DiagnosticRecordQuery query,
        CancellationToken cancellationToken = default);
}