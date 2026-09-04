
namespace UnityCommander.Diagnostics.Tracing
{
    public sealed record DiagnosticTraceEvent
    {
        public required DateTime Timestamp { get; init; }

        public required string TraceId { get; init; }

        public required string Source { get; init; }

        public required string EventName { get; init; }

        public int Depth { get; init; }

        public IReadOnlyDictionary<string, object?> Data { get; init; }
            = new Dictionary<string, object?>();
    }
}
