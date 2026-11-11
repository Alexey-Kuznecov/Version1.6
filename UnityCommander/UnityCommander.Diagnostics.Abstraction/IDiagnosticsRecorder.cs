
namespace UnityCommander.Diagnostics.Abstraction;

public interface IDiagnosticsRecorder : IAsyncDisposable
{
    DiagnosticSession? CurrentSession { get; }

    ValueTask<DiagnosticSession> StartSessionAsync(
        DiagnosticSessionInfo info,
        CancellationToken cancellationToken = default);

    bool TryRecord(DiagnosticRecord record);

    ValueTask FlushAsync(
        CancellationToken cancellationToken = default);

    ValueTask StopSessionAsync(
        CancellationToken cancellationToken = default);
}
