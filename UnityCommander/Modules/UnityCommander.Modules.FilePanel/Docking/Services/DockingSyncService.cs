
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Docking;
using UnityCommander.Common.State;
using UnityCommander.Modules.FilePanel.Docking.Builders;
using UnityCommander.Modules.FilePanel.Docking.Diff;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;
using UnityCommander.Services;
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
        private ITabRegistry _tabRegistry;
        private DockingSyncContext _dockingSyncContext;
        private DockingManager _manager;
        private DockingSnapshot _previous;
        public event Action<DiffResult> OnDiff;

        private bool _initialized;

        public DockingSyncService(
            DockingSyncContext syncContext,
            IPanelRegistry panelRegistry,
            ITabRegistry tabRegistry,
            IDockingService dockingService = null,
            IDockingDiffEngine diffEngine = null,
            IDockingSnapshotBuilder snapshotBuilder = null)
        {
            _manager = dockingService.GetDockingManager();
            _dockingSyncContext = syncContext;
            _builder = snapshotBuilder ?? new DockingSnapshotBuilder(dockingService, panelRegistry, syncContext);
            _diff = diffEngine ?? new DockingDiffEngine();
            _panelRegistry = panelRegistry;
            _tabRegistry = tabRegistry;
        }

        public void Initialize(List<PanelSessionState> panels)
        {
            HydrateLayoutIds();
            RestorePanels(panels);
            RestoreFloatingWindows();
            RestoreActiveState(panels);
            FinalizeInitialization();
        }

        private void HydrateLayoutIds()
        {
            foreach (var doc in _manager.Layout.Descendents().OfType<LayoutDocument>())
            {
                if (!string.IsNullOrEmpty(doc.ContentId))
                    _dockingSyncContext.GetOrCreateTabId(doc);
            }
        }

        private void RestorePanels(List<PanelSessionState> panels)
        {
            var panes = _manager.Layout.Descendents().OfType<LayoutDocumentPane>();

            foreach (var pane in panes)
            {
                var state = FindPanelStateForPane(pane, panels);
                if (state == null) continue;

                _panelRegistry.EnsurePanel(state.PanelId);
                _dockingSyncContext.Register(pane, state.PanelId);

                foreach (var tab in state.Tabs)
                    _panelRegistry.AddTab(state.PanelId, tab.TabId);
            }
        }

        private void RestoreFloatingWindows()
        {
            var floating = _manager.Layout.FloatingWindows
                .OfType<LayoutDocumentFloatingWindow>()
                .ToList();

            foreach (var fw in floating)
            {
                var docs = fw.Descendents().OfType<LayoutDocument>();

                foreach (var doc in docs)
                {
                    if (!Guid.TryParse(doc.ContentId, out var tabId))
                        continue;

                    // 1. получить VM
                    var vm = _tabRegistry.GetTab(tabId);

                    if (vm == null)
                        continue;

                    // 2. убедиться что панель существует
                    var panelId = _dockingSyncContext.EnsurePaneForFloatingWindow(fw);

                    _panelRegistry.EnsurePanel(panelId);

                    // 3. register mapping
                    _dockingSyncContext.Register(fw, panelId);

                    // 4. ВАЖНО: attach VM → View будет позже, но VM уже жива
                    vm.OnAttached(doc /* или view через callback */);
                }
            }
        }

        private void RestoreActiveState(List<PanelSessionState> panels)
        {
            foreach (var pane in _manager.Layout.Descendents().OfType<LayoutDocumentPane>())
            {
                var state = FindPanelStateForPane(pane, panels);
                if (state == null) continue;

                var first = state.Tabs.FirstOrDefault();
                if (first.TabId == Guid.Empty) continue;

                _panelRegistry.SetActiveTab(state.PanelId, first.TabId);
                _panelRegistry.SetActivePanel(state.PanelId);
            }
        }

        private void FinalizeInitialization()
        {
            _previous = _builder.Build();
            _initialized = true;

            _manager.ActiveContentChanged += (_, __) => HandleLayoutChanged(_, __);
        }

        private PanelSessionState FindPanelStateForPane(
            LayoutDocumentPane pane,
            List<PanelSessionState> states)
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
