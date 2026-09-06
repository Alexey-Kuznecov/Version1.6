

using UnityCommander.CLI.Core;
using UnityCommander.Diagnostics.Reporting;

namespace UnityCommander.Commands.Diagnostic
{
    public interface IDiagnosticRender
    {
        void Render(IConsoleOutput output, DiagnosticResult result);
    }
}
