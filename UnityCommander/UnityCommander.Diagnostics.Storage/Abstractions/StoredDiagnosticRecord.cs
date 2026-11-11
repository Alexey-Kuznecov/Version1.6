
using UnityCommander.Diagnostics.Abstraction;

namespace UnityCommander.Diagnostics.Storage;

public sealed record StoredDiagnosticRecord(
    Guid Id,
    Guid SessionId,
    DiagnosticRecordKind Kind,
    DateTimeOffset Timestamp,
    string Source,
    string Name,
    string? PluginId,
    long? DurationTicks,
    string PayloadJson);

