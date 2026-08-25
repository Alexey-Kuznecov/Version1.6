
namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticSource : IDiagnostic
    {
        object GetState();

        string Describe();
    }
}
