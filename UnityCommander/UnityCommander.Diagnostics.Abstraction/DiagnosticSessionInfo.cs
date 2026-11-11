
namespace UnityCommander.Diagnostics.Abstraction;

public sealed record DiagnosticSessionInfo(
    string Name,
    string ApplicationVersion,
    string? GitCommit,
    string? Configuration);

