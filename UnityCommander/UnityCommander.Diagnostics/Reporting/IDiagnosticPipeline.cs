
using UnityCommander.Common.Diagnostic;

namespace UnityCommander.Diagnostics.Reporting
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
