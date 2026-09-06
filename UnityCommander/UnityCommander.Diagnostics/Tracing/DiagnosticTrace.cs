
namespace UnityCommander.Diagnostics.Tracing
{
    public sealed class DiagnosticTrace : IDiagnosticTrace
    {
        private readonly IDiagnosticTraceStore _store;

        public DiagnosticTrace(IDiagnosticTraceStore store)
        {
            _store = store;
        }

        public void Write(
            string source,
            string eventName,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            var entry = new DiagnosticTraceEvent
            {
                Timestamp = DateTime.UtcNow,
                TraceId = Guid.NewGuid().ToString("N"),
                Source = source,
                EventName = eventName,
                Data = data ?? EmptyData
            };

            _store.Add(entry);
        }

        public IDiagnosticTraceScope Begin(
            string source,
            string operation,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            return new DiagnosticTraceScope(
                this,
                source,
                operation,
                data);
        }

        internal void Write(
            string traceId,
            string source,
            string eventName,
            int depth,
            IReadOnlyDictionary<string, object?>? data = null)
        {
            _store.Add(new DiagnosticTraceEvent
            {
                Timestamp = DateTime.UtcNow,
                TraceId = traceId,
                Source = source,
                EventName = eventName,
                Depth = depth,
                Data = data ?? EmptyData
            });
        }

        private static readonly IReadOnlyDictionary<string, object?> EmptyData =
            new Dictionary<string, object?>();
    }
}
