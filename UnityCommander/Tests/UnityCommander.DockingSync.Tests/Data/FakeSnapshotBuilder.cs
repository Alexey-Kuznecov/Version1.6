
using UnityCommander.Modules.FilePanel.Docking.Builders;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;

namespace UnityCommander.DockingSync.Tests.Data
{
    public class FakeSnapshotBuilder : IDockingSnapshotBuilder
    {
        private DockingSnapshot _snapshot;

        public void Set(DockingSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public DockingSnapshot Build() => _snapshot;
    }
}
