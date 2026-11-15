
namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnostic
    {
        string Name { get; }

        string Subsystem => Name;

        DiagnosticCardinality Cardinality { get; }
    }
}
