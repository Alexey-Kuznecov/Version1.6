
using AvalonDock;
using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Services.Docking;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public sealed class VisibleTabResolver : IVisibleTabResolver
    {
        private readonly DockingSyncContext _dockingSyncContext;
        private readonly DockingManager _manager;

        public VisibleTabResolver(
            DockingSyncContext dockingSyncContext,
            IDockingService dockingService)
        {
            _dockingSyncContext = dockingSyncContext;
            _manager = dockingService.GetDockingManager();
        }

        public IReadOnlyCollection<Guid> GetVisibleTabs()
        {
            var panes = GetDocumentPanes();
            var (activePane, activeDocument) = GetActiveDocument(panes);

            return panes
                .Select(pane => GetVisibleDocument(
                    pane,
                    activePane,
                    activeDocument))
                .Select(GetTabId)
                .Where(tabId => tabId.HasValue)
                .Select(tabId => tabId!.Value)
                .ToList();
        }

        private IReadOnlyCollection<LayoutDocumentPane> GetDocumentPanes()
        {
            return _manager.Layout
                .Descendents()
                .OfType<LayoutDocumentPane>()
                .ToList();
        }

        private static (
            LayoutDocumentPane? Pane,
            LayoutDocument? Document)
            GetActiveDocument(
                IEnumerable<LayoutDocumentPane> panes)
        {
            foreach (var pane in panes)
            {
                var document = pane.Children
                    .OfType<LayoutDocument>()
                    .FirstOrDefault(x => x.IsActive);

                if (document != null)
                    return (pane, document);
            }

            return (null, null);
        }

        private static LayoutDocument? GetVisibleDocument(
            LayoutDocumentPane pane,
            LayoutDocumentPane? activePane,
            LayoutDocument? activeDocument)
        {
            if (pane == activePane)
                return activeDocument;

            return pane.SelectedContent as LayoutDocument;
        }

        private Guid? GetTabId(LayoutDocument? document)
        {
            if (document == null ||
                string.IsNullOrEmpty(document.ContentId))
            {
                return null;
            }

            return TryGetTabId(document, out var tabId)
                ? tabId
                : null;
        }

        private static bool TryGetTabId(
            LayoutDocument document,
            out Guid tabId)
        {
            return Guid.TryParse(
                document.ContentId,
                out tabId);
        }
    }
}
