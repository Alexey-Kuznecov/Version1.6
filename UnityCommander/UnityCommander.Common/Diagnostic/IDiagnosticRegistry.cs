
using System.Collections.Generic;

namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticRegistry
    {
        void Register(IDiagnostic diagnostic);

        DiagnosticDefinition Get(string name);

        IEnumerable<DiagnosticDefinition> GetAll();

        public bool TryGet(
            string name,
            out DiagnosticDefinition? diagnostic);
    }
}
