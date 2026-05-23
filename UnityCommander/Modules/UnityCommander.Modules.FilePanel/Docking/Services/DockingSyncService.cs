
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Docking;
using UnityCommander.Common.State;
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

        private bool _initialized;

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

        public void Initialize(List<PanelState> panels)
        {
            foreach (var doc in _manager.Layout.Descendents().OfType<LayoutDocument>())
            {
                if (string.IsNullOrEmpty(doc.ContentId))
                    continue;

                _dockingSyncContext.GetOrCreateTabId(doc);
            }

            var panes = _manager.Layout.Descendents().OfType<LayoutDocumentPane>().ToList();

            foreach (var pane in panes)
            {
                var state = FindPanelStateForPane(pane, panels);

                if (state == null)
                    continue;

                _panelRegistry.EnsurePanel(state.PanelId);

                _dockingSyncContext.Register(pane, state.PanelId);

                foreach (var tab in state.Tabs)
                {
                    _panelRegistry.AddTab(state.PanelId, tab.TabId);
                }

                var firstTab = state.Tabs.FirstOrDefault();
                if (firstTab.TabId != Guid.Empty)
                {
                    _panelRegistry.SetActiveTab(state.PanelId, firstTab.TabId);
                    _panelRegistry.SetActivePanel(state.PanelId);
                }
            }

            _previous = _builder.Build();

            _initialized = true;

            _manager.ActiveContentChanged += (_, __) => HandleLayoutChanged(_,__);
        }

        private PanelState FindPanelStateForPane(
            LayoutDocumentPane pane,
            List<PanelState> states)
        {
            var paneTabs = pane.Children
                .OfType<LayoutDocument>()
                .Select(d => Guid.Parse(d.ContentId))
                .ToHashSet();

            return states.FirstOrDefault(state =>
                state.Tabs.Any(t => paneTabs.Contains(t.TabId)));
        }

        public void HandleLayoutChanged(object sender, EventArgs e)
        {
            if (!_initialized)
                return;

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
            return diff.Operations.Any();
        }
    }
}
