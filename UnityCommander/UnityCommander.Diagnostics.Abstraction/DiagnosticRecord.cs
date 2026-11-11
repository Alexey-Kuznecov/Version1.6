
namespace UnityCommander.Diagnostics.Abstraction;

public sealed record DiagnosticRecord(
    DiagnosticRecordKind Kind,
    DateTimeOffset Timestamp,
    string Source,
    string Name,
    object? Payload,
    string? PluginId = null,
    TimeSpan? Duration = null);

