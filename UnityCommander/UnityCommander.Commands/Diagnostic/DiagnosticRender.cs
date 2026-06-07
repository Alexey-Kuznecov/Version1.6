
using System.Collections.Generic;
using UnityCommander.CLI.Core;
using UnityCommander.Commands.Rendering;

namespace UnityCommander.Commands.Diagnostic
{
    public class DiagnosticRender : IDiagnosticRender
    {
        public void Render(
            IConsoleOutput output,
            DiagnosticResult result)
        {
            var dictionary = result.Values as Dictionary<string, object>;

            foreach (var item in dictionary)
            {
                output.WriteLine(
                    $"{item.Key}: {item.Value}");
            }
        }
    }
}
