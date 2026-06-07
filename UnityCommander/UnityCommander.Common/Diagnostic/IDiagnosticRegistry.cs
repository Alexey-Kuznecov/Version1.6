
using System.Collections.Generic;

namespace UnityCommander.Common.Diagnostic
{
    public interface IDiagnosticRegistry
    {
        void Register(IDiagnosticSource source);

        bool TryGet(string name, out IDiagnosticSource? source);

        IDiagnosticSource Get(string source);

        IEnumerable<IDiagnosticSource> GetAll();
    }
}
