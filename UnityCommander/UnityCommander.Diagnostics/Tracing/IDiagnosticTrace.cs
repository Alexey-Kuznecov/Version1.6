
namespace UnityCommander.Diagnostics.Tracing
{
    public interface IDiagnosticTrace
    {
        void Write(
            string source,
            string eventName,
            IReadOnlyDictionary<string, object?>? data = null);

        IDiagnosticTraceScope Begin(
            string source,
            string operation,
            IReadOnlyDictionary<string, object?>? data = null);
    }
}
