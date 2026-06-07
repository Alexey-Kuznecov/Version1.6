
using UnityCommander.DockingSync.Tests.Data;
using UnityCommander.Modules.FilePanel.Docking.Diff;
using UnityCommander.Modules.FilePanel.Docking.Services;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;
using UnityCommander.Services;
using UnityCommander.Services.Docking;

namespace UnityCommander.DockingSync.Tests
{
    public class PanelDiffSystemStressTests
    {
        [Fact]
        public void SyncService_Should_Add_Tab_To_Model()
        {
            var ctx = new DockingSyncContext();
            var builder = new FakeSnapshotBuilder();
            var diff = new DockingDiffEngine();
            var panelRegistry = new PanelRegistry(); // реальный

            //var service = new DockingSyncService(ctx, panelRegistry, null, diff, builder);

            var panelId = Guid.NewGuid();
            var tab1 = Guid.NewGuid();
            var tab2 = Guid.NewGuid();

            // стартовое состояние
            builder.Set(new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panelId,
                        Tabs = new() { tab1 }
                    }
                }
            });

            //service.Initialize();

            // новое состояние (добавили вкладку)
            builder.Set(new DockingSnapshot
            {
                Panels = new()
                {
                    new PanelSnapshot
                    {
                        PanelId = panelId,
                        Tabs = new() { tab1, tab2 }
                    }
                }
            });

            //service.HandleLayoutChanged(null, EventArgs.Empty);

            var panel = panelRegistry.GetPanel(panelId);

            Assert.Contains(tab2, panel.Tabs);
        }
    }
}
