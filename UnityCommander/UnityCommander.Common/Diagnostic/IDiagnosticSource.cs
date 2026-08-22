
namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticSource
    {
        string Name { get; }

        object GetState();

        string Describe();
    }
}
