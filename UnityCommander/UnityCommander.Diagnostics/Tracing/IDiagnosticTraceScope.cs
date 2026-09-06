
namespace UnityCommander.Diagnostics.Tracing
{
    public interface IDiagnosticTraceScope : IDisposable
    {
        string Id { get; }

        void Write(
            string eventName,
            IReadOnlyDictionary<string, object?>? data = null);

        void Complete(
            IReadOnlyDictionary<string, object?>? data = null);

        void Fail(
            Exception exception,
            IReadOnlyDictionary<string, object?>? data = null);
    }
}
