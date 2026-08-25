namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticReporter : IDiagnostic
    {
        void Report(IDiagnosticWriter writer);
    }
}