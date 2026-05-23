
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

        public DockingSnapshotBuilder(IDockingService docking, DockingSyncContext syncContext)
        {
            _docking = docking;
            _syncContext = syncContext;
        }

        public DockingSnapshot Build()
        {
            var manager = _docking.GetDockingManager();
            var layout = manager?.Layout;

            var snapshot = new DockingSnapshot();

            if (layout == null)
                return snapshot;

            var panes = GetAllDocumentPanes(layout);

            foreach (var pane in panes)
            {
                var doc = default(LayoutDocument);
                for (int i = 0; i < pane.Children.Count; i++)
                {
                    doc = pane.Children[i] as LayoutDocument;
                }
                var panelId = _syncContext.GetPaneId(pane);

                if (!Guid.TryParse(doc.ContentId, out var tabId))
                    continue;

                var panel = new PanelSnapshot
                {
                    PanelId = panelId,
                    Tabs = pane.Children
                        .OfType<LayoutDocument>()
                        .Select(d => Guid.TryParse(d.ContentId, out var id) ? id : (Guid?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList()
                };

                snapshot.Panels.Add(panel);
            }

            return snapshot;
        }

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
