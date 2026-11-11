
namespace UnityCommander.Diagnostics.Abstraction;

public sealed record DiagnosticRecordEntry(
    Guid Id,
    Guid SessionId,
    DiagnosticRecordKind Kind,
    DateTimeOffset Timestamp,
    string Source,
    string Name,
    string? PluginId,
    long? DurationTicks,
    string PayloadJson);