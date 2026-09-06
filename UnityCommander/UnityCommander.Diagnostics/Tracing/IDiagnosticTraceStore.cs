
namespace UnityCommander.Diagnostics.Tracing
{
    public interface IDiagnosticTraceStore
    {
        void Add(DiagnosticTraceEvent entry);

        IReadOnlyList<DiagnosticTraceEvent> GetSnapshot();

        IReadOnlyList<DiagnosticTraceEvent> Query(
            DiagnosticTraceQuery query);

        void Clear();
    }
}
