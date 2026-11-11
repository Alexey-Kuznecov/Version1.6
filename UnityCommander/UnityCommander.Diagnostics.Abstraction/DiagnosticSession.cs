
namespace UnityCommander.Diagnostics.Abstraction;

public sealed record DiagnosticSession(
    Guid Id,
    string Name,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    string ApplicationVersion,
    string? GitCommit,
    string? Configuration);
