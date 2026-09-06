
namespace UnityCommander.Common.Diagnostic
{
    public sealed class DiagnosticRegistration
    {
        public required string Id { get; init; }

        public required IDiagnostic Diagnostic { get; init; }

        public IDiagnosticSource? Source =>
            Diagnostic as IDiagnosticSource;

        public IDiagnosticReporter? Reporter =>
            Diagnostic as IDiagnosticReporter;
    }
}
