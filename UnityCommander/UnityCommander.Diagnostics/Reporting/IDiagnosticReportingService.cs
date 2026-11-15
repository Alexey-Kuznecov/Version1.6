
namespace UnityCommander.Diagnostics.Reporting
{
    public interface IDiagnosticReportingService
    {
        bool Publish(DiagnosticEvent diagnosticEvent);
    }
}
