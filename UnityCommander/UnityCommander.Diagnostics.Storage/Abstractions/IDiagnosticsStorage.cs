
using UnityCommander.Diagnostics.Abstraction;

namespace UnityCommander.Diagnostics.Storage;

public interface IDiagnosticsStorage : IAsyncDisposable
{
    ValueTask InitializeAsync(
        CancellationToken cancellationToken = default);

    ValueTask CreateSessionAsync(
        DiagnosticSession session,
        CancellationToken cancellationToken = default);

    ValueTask WriteBatchAsync(
        IReadOnlyList<StoredDiagnosticRecord> records,
        CancellationToken cancellationToken = default);

    ValueTask CompleteSessionAsync(
        Guid sessionId,
        DateTimeOffset completedAt,
        CancellationToken cancellationToken = default);
}

