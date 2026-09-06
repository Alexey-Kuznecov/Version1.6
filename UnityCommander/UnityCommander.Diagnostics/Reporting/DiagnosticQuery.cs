
namespace UnityCommander.Diagnostics.Reporting
{
    public sealed class DiagnosticQuery
    {
        public string? Source { get; init; }
        public string? Format { get; init; }
        public DiagnosticMode Mode { get; init; }
        public Dictionary<string, string>? Filters { get; init; }
    }
}
