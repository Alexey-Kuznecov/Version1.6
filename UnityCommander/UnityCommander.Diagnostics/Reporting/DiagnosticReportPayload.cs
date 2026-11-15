
namespace UnityCommander.Diagnostics.Reporting
{
    public sealed record DiagnosticReportPayload(
      object? EventPayload,
      IReadOnlyList<DiagnosticComponentReport> Components);

    public sealed record DiagnosticComponentReport(
        string Name,
        object? State,
        string? Error);
}
