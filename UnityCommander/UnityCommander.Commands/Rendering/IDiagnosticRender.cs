
using UnityCommander.CLI.Core;
using UnityCommander.Commands.Diagnostic;

namespace UnityCommander.Commands.Rendering
{
    public interface IDiagnosticRender
    {
        void Render(IConsoleOutput output, DiagnosticResult result);
    }
}
