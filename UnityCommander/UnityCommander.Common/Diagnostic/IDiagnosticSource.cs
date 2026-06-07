
using System.Collections.Generic;

namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticSource
    {
        string Name { get; }

        IReadOnlyDictionary<string, object?> GetState();

        string Describe();
    }
}
