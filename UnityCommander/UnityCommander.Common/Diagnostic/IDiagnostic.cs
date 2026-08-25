
namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnostic
    {
        string Name { get; }

        DiagnosticCardinality Cardinality { get; }
    }
}
