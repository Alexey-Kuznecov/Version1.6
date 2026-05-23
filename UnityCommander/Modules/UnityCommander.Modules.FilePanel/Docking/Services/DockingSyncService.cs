
using System;
using System.Linq;
using UnityCommander.Common.Docking;
using UnityCommander.Modules.FilePanel.Docking.Builders;
using UnityCommander.Modules.FilePanel.Docking.Diff;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander.Modules.FilePanel.Docking.Services
{
    public class DockingSyncService : IDockingSyncService
    {
        private IDockingDiffEngine _diff;
        private IDockingSnapshotBuilder _builder;
        private IPanelRegistry _panelRegistry;
        private DockingSyncContext _dockingSyncContext;
        private DockingManager _manager;

        private DockingSnapshot _previous;
        public event Action<DiffResult> OnDiff;

        public DockingSyncService(
            DockingSyncContext syncContext,
            IPanelRegistry panelRegistry,
            IDockingService dockingService = null,
            IDockingDiffEngine diffEngine = null,
            IDockingSnapshotBuilder snapshotBuilder = null)
        {
            _manager = dockingService.GetDockingManager();
            _dockingSyncContext = syncContext;
            _builder = snapshotBuilder ?? new DockingSnapshotBuilder(dockingService, syncContext);
            _diff = diffEngine ?? new DockingDiffEngine();
            _panelRegistry = panelRegistry;
        }

        public void Initialize()
        {
            Bootstrap(); // читаем уже созданный layout
            _manager.ActiveContentChanged += (_, __) => HandleLayoutChanged(_,__);
        }

        private void Bootstrap()
        {
            foreach (var doc in _manager.Layout.Descendents().OfType<LayoutDocument>())
            {
                if (string.IsNullOrEmpty(doc.ContentId))
                    continue;

                _dockingSyncContext.GetOrCreateTabId(doc);
            }

            _previous = _builder.Build();

            foreach (var panel in _previous.Panels)
            {
                _panelRegistry.EnsurePanel(panel.PanelId);

                foreach (var tabId in panel.Tabs)
                {
                    _panelRegistry.AddTab(panel.PanelId, tabId);
                }

                // 👉 можно задать активную вкладку (например первую)
                var firstTab = panel.Tabs.FirstOrDefault();
                if (firstTab != Guid.Empty)
                {
                    _panelRegistry.SetActiveTab(panel.PanelId, firstTab);
                    _panelRegistry.SetActivePanel(panel.PanelId);
                }
            }
        }

        public void HandleLayoutChanged(object sender, EventArgs e)
        {
            var current = _builder.Build();

            var diff = _diff.Diff(_previous, current);

            if (HasChanges(diff))
            {
                OnDiff?.Invoke(diff);
            }

            _previous = current;
        }

        private bool HasChanges(DiffResult diff)
        {
            return diff.AddedTabs.Any()
                || diff.RemovedTabs.Any()
                || diff.MovedTabs.Any();
        }
    }
}
