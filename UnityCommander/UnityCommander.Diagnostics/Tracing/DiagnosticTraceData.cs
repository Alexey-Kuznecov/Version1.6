
namespace UnityCommander.Diagnostics.Tracing
{
    public static class DiagnosticTraceData
    {
        public static IReadOnlyDictionary<string, object?> Of(
            params (string Key, object? Value)[] values)
        {
            return values.ToDictionary(
                x => x.Key,
                x => x.Value);
        }
    }
}
