
namespace UnityCommander.Diagnostics.Reporting
{
    public sealed record DiagnosticEvent(
     string Subsystem,
     string Name,
     object? Payload = null,
     string? PluginId = null,
     TimeSpan? Duration = null)
    {
        public DateTimeOffset Timestamp { get; init; }
            = DateTimeOffset.UtcNow;
    }
}
