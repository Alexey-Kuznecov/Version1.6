
namespace UnityCommander.Diagnostics.Tracing
{
    using System.Threading;

    public sealed class DiagnosticTraceContext
    {
        private int _isActive = 1;

        public DiagnosticTraceContext(
            string traceId,
            string spanId,
            string? parentSpanId,
            int depth,
            DiagnosticTraceContext? parentContext)
        {
            TraceId = traceId;
            SpanId = spanId;
            ParentSpanId = parentSpanId;
            Depth = depth;
            ParentContext = parentContext;
        }

        public string TraceId { get; }

        public string SpanId { get; }

        public string? ParentSpanId { get; }

        public int Depth { get; }

        public DiagnosticTraceContext? ParentContext { get; }

        public bool IsActive =>
            Volatile.Read(ref _isActive) == 1;

        public void Deactivate()
        {
            Interlocked.Exchange(ref _isActive, 0);
        }
    }
}
