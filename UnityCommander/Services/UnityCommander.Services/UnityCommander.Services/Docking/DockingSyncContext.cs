
using System;
using System.Collections.Generic;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander.Services.Docking
{
    public class DockingSyncContext
    {
        private readonly Dictionary<LayoutDocument, Guid> _tabs = new();
        private readonly Dictionary<LayoutDocumentPane, Guid> _panes = new();
        private readonly Dictionary<LayoutDocumentFloatingWindow, Guid> _floating = new();

        public void Register(LayoutDocument doc, Guid id)
        {
            _tabs[doc] = id;
        }

        public void Register(LayoutDocumentPane pane, Guid id)
        {
            _panes[pane] = id;
        }

        public void Register(LayoutDocumentFloatingWindow window, Guid id)
        {
            _floating[window] = id;
        }

        public Guid GetTabId(LayoutDocument doc) => _tabs[doc];
      
        public Guid GetOrCreatePaneId(LayoutDocumentPane pane)
        {
            if (_panes.TryGetValue(pane, out var id))
                return id;

            id = Guid.NewGuid();
            _panes[pane] = id;

            return id;
        }

        public Guid EnsurePaneForFloatingWindow(LayoutDocumentFloatingWindow window)
        {
            if (_floating.TryGetValue(window, out var id))
                return id;

            id = Guid.NewGuid();
            _floating[window] = id;

            return id;
        }

        public Guid GetOrCreateWindowId(LayoutDocumentFloatingWindow window)
        {
            if (!_floating.TryGetValue(window, out var id))
            {
                if (!Guid.TryParse(window.RootDocument.ContentId, out id))
                {
                    id = Guid.NewGuid();
                    window.RootDocument.ContentId = id.ToString(); // 💥 ВАЖНО
                }

                _floating[window] = id;
            }

            return id;
        }

        public Guid GetOrCreateTabId(LayoutDocument doc)
        {
            if (!_tabs.TryGetValue(doc, out var id))
            {
                if (!Guid.TryParse(doc.ContentId, out id))
                {
                    id = Guid.NewGuid();
                    doc.ContentId = id.ToString(); // 💥 ВАЖНО
                }

                _tabs[doc] = id;
            }

            return id;
        }

        public void Remove(LayoutDocumentPane pane)
        {
            if (!_panes.TryGetValue(pane, out var id))
             return;

            _panes.Remove(pane); 
        }
    }
}
