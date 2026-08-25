
using UnityCommander.Common.Diagnostic;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Core.Diagnostics
{
    public sealed class SelectionDiagnostics // : IDiagnosticReporter
    {
        //private readonly ISelectionManagerRegistry _registry;

        public string Name => "selection";

        public SelectionDiagnostics(IEnumerable<ISelectionManager> selections, IDiagnosticRegistry diagnostic)
        {
            //diagnostic.Register(this);
        }

        public void Report(IDiagnosticWriter writer)
        {
            //foreach (var manager in _registry.GetAll())
            //{
            //    writer.WriteLine(
            //        $"Selection: {manager.SelectedItems.Count}");
            //}
        }
    }
}
