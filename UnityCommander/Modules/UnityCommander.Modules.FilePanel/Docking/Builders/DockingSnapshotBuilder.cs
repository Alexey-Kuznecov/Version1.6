
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Modules.FilePanel.Docking.Snapshot;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander.Modules.FilePanel.Docking.Builders
{
    public class DockingSnapshotBuilder : IDockingSnapshotBuilder
    {
        private readonly IDockingService _docking;
        private readonly DockingSyncContext _syncContext;
        private IPanelRegistry _panelRegistry;

        public DockingSnapshotBuilder(
            IDockingService docking, 
            IPanelRegistry panelRegistry,
            DockingSyncContext syncContext)
        {
            _docking = docking;
            _syncContext = syncContext;
            _panelRegistry = panelRegistry;
        }

        public DockingSnapshot Build()
        {
            CleanupEmptyPanes();

            var locations = BuildTabLocations();

            var snapshot = new DockingSnapshot();

            foreach (var group in locations.GroupBy(x => x.PanelId))
            {
                snapshot.Panels.Add(new PanelSnapshot
                {
                    PanelId = group.Key,
                    IsFloating = group.First().HostType == TabHostType.Floating,
                    Tabs = group
                        .Select(x => x.TabId)
                        .ToList()
                });
            }

            return snapshot;
        }

        private List<TabLocation> BuildTabLocations()
        {
            var layout = _docking.GetDockingManager()?.Layout;
            if (layout == null)
                return new();

            var result = new List<TabLocation>();

            foreach (var doc in layout.Descendents().OfType<LayoutDocument>())
            {
                if (!Guid.TryParse(doc.ContentId, out var tabId))
                    continue;

                var host = ResolveHost(doc); // 👈 вот ключ

                result.Add(new TabLocation
                {
                    TabId = tabId,
                    PanelId = host.PanelId,
                    HostType = host.Type
                });
            }

            return result;
        }

        private (Guid PanelId, TabHostType Type) ResolveHost(LayoutDocument doc)
        {
            if (doc.FindParent<LayoutDocumentFloatingWindow>() is LayoutDocumentFloatingWindow fw)
            {
                var panelId = _syncContext.GetOrCreateWindowId(fw);

                _panelRegistry.EnsurePanel(panelId);

                return (panelId, TabHostType.Floating);
            }

            var pane = doc.FindParent<LayoutDocumentPane>();

            if (pane == null)
                throw new InvalidOperationException(
                    $"Pane not found for document '{doc.ContentId}'.");

            var dockPanelId = _syncContext.GetOrCreatePaneId(pane);

            _panelRegistry.EnsurePanel(dockPanelId);

            return (dockPanelId, TabHostType.Docked);
        }

        private void CleanupEmptyPanes()
        {
            var layout = _docking.GetDockingManager()?.Layout;

            if (layout == null)
                return;

            foreach (var pane in GetAllDocumentPanes(layout))
            {
                var hasTabs = pane.Children
                    .OfType<LayoutDocument>()
                    .Any();

                if (!hasTabs)
                {
                    _syncContext.Remove(pane);
                }
            }
        }

        //public DockingSnapshot Build()
        //{
        //    var manager = _docking.GetDockingManager();
        //    var layout = manager?.Layout;

        //    var snapshot = new DockingSnapshot();

        //    if (layout == null)
        //        return snapshot;

        //    var panes = GetAllDocumentPanes(layout);

        //    var floatingWindows = layout.FloatingWindows.OfType<LayoutDocumentFloatingWindow>();

        //    foreach (var fw in floatingWindows)
        //    {
        //        var docs = fw.Descendents()
        //            .OfType<LayoutDocument>();

        //        foreach (var doc in docs)
        //        {
        //            if (!Guid.TryParse(doc.ContentId, out var tabId))
        //                continue;

        //            var panelId = _syncContext.GetOrCreateWindowId(fw);

        //            snapshot.Panels.Add(new PanelSnapshot
        //            {
        //                PanelId = panelId,
        //                Tabs = new List<Guid> { tabId },
        //                IsFloating = true
        //            });
        //        }
        //    }

        //    foreach (var pane in panes)
        //    {
        //        var doc = default(LayoutDocument);
        //        for (int i = 0; i < pane.Children.Count; i++)
        //        {
        //            doc = pane.Children[i] as LayoutDocument;
        //        }

        //        var tabs = pane.Children
        //            .OfType<LayoutDocument>()
        //            .Select(d => Guid.TryParse(d.ContentId, out var id) ? id : (Guid?)null)
        //            .Where(id => id.HasValue)
        //            .Select(id => id.Value)
        //            .ToList();

        //        if (tabs.Count == 0)
        //        {
        //            _syncContext.Remove(pane);
        //            continue;
        //        }

        //        var panelId = _syncContext.GetOrCreatePaneId(pane);

        //        _panelRegistry.EnsurePanel(panelId);

        //        if (!Guid.TryParse(doc.ContentId, out var tabId))
        //            continue;

        //        var panel = new PanelSnapshot
        //        {
        //            PanelId = panelId,
        //            Tabs = pane.Children
        //                .OfType<LayoutDocument>()
        //                .Select(d => Guid.TryParse(d.ContentId, out var id) ? id : (Guid?)null)
        //                .Where(id => id.HasValue)
        //                .Select(id => id.Value)
        //                .ToList()
        //        };

        //        snapshot.Panels.Add(panel);
        //    }

        //    return snapshot;
        //}


        // 🔥 Рекурсивный обход layout дерева
        private IEnumerable<LayoutDocumentPane> GetAllDocumentPanes(ILayoutElement root)
        {
            if (root == null)
                yield break;

            if (root is LayoutDocumentPane pane)
                yield return pane;

            if (root is ILayoutContainer container)
            {
                foreach (var child in container.Children)
                {
                    foreach (var nested in GetAllDocumentPanes(child))
                        yield return nested;
                }
            }
        }
    }
}
