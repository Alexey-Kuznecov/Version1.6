
using UnityCommander.Common.Docking;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;

namespace UnityCommander.Modules.FilePanel.Docking.Diff
{
    public interface IDockingDiffEngine
    {
        DiffResult Diff(DockingSnapshot oldSnap, DockingSnapshot newSnap);
    }
}
