
namespace UnityCommander.Diagnostics.Tracing
{
    public sealed class DiagnosticTraceQuery
    {
        public string? Source { get; init; }

        public string? EventName { get; init; }

        public string? TraceId { get; init; }

        public DateTime? From { get; init; }

        public DateTime? To { get; init; }

        public IReadOnlyDictionary<string, object?>? Data { get; init; }

        public int? Limit { get; init; }
    }
}
