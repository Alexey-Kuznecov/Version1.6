namespace UnityCommander.Commands.Diagnostic
{
    public interface IDiagnosticPipeline
    {
        DiagnosticResult Execute(DiagnosticQuery query);
    }
}
