

using UnityCommander.CLI.Core;
using UnityCommander.Diagnostics.Diagnostic;

namespace UnityCommander.Commands.Diagnostic
{
    public interface IDiagnosticRender
    {
        void Render(IConsoleOutput output, DiagnosticResult result);
    }
}
