namespace UnityCommander.Diagnostics.Abstraction;

public sealed record DiagnosticRecordQuery(
    Guid SessionId,
    DiagnosticRecordKind? Kind = null,
    string? Source = null,
    string? Name = null,
    string? PluginId = null,
    DateTimeOffset? From = null,
    DateTimeOffset? To = null,
    int Limit = 5000,
    int Offset = 0);