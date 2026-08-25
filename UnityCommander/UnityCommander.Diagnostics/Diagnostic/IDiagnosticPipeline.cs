
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Diagnostics.Diagnostic
{
    public interface IDiagnosticPipeline
    {
        DiagnosticResult Execute(DiagnosticQuery query);

        void Report(DiagnosticQuery query, IDiagnosticWriter writer);

        bool ReportChanged(
            DiagnosticQuery query,
            IDiagnosticWriter writer);

        //IReadOnlyList<DiagnosticResult> ExecuteAll(DiagnosticQuery query);
    }
}
