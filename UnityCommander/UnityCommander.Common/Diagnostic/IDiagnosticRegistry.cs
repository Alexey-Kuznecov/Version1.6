
using System;
using System.Collections.Generic;

namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticRegistry
    {
        string Register(IDiagnostic diagnostic);

        DiagnosticDefinition Get(string name);

        IEnumerable<DiagnosticDefinition> GetAll();

        void Unregister(IDiagnostic diagnostic);

        public bool TryGet(
            string name,
            out DiagnosticDefinition? diagnostic);
    }
}
