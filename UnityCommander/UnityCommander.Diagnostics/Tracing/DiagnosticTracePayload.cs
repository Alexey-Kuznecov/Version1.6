
namespace UnityCommander.Diagnostics.Tracing
{
    public sealed record DiagnosticTracePayload(
     string TraceId,
     string? SpanId,
     string? ParentSpanId,
     int Depth,
     IReadOnlyDictionary<string, object?> Data);
}
