
using System.Text;

namespace UnityCommander.Diagnostics.Tracing
{
    public sealed record DiagnosticTraceEvent
    {
        public required DateTime Timestamp { get; init; }

        public required string TraceId { get; init; }

        public string? SpanId { get; init; }

        public string? ParentSpanId { get; init; }

        public required string Source { get; init; }

        public required string EventName { get; init; }

        public int Depth { get; init; }

        public long? DurationTicks { get; init; }

        public IReadOnlyDictionary<string, object?> Data { get; init; }
            = new Dictionary<string, object?>();

        public override string ToString()
        {
            var builder = new StringBuilder();

            var indent = new string(' ', Depth * 4);

            builder.Append(indent);
            builder.Append('[');
            builder.Append(Timestamp.ToLocalTime().ToString("HH:mm:ss.fff"));
            builder.Append("] ");

            builder.Append(Source);
            builder.Append("  ");
            builder.Append(EventName);

            if (DurationTicks.HasValue)
            {
                builder.Append(" (");
                builder.Append(
                    TimeSpan.FromTicks(DurationTicks.Value).TotalMilliseconds
                        .ToString("N2"));
                builder.Append(" ms)");
            }

            foreach (var pair in Data)
            {
                builder.AppendLine();
                builder.Append(indent);
                builder.Append("    ");
                builder.Append(pair.Key);
                builder.Append(" = ");
                builder.Append(pair.Value ?? "null");
            }

            return builder.ToString();
        }
    }
}
