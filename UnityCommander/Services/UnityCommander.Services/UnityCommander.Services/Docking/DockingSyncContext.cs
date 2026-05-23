
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using Xceed.Wpf.AvalonDock.Layout;

namespace UnityCommander.Services.Docking
{
    public class DockingSyncContext
    {
        private readonly Dictionary<LayoutDocument, Guid> _tabs = new();
        private readonly Dictionary<LayoutDocumentPane, Guid> _panes = new();

        public void Register(LayoutDocument doc, Guid id)
        {
            _tabs[doc] = id;
        }

        public void Register(LayoutDocumentPane pane, Guid id)
        {
            _panes[pane] = id;
        }

        public Guid GetTabId(LayoutDocument doc) => _tabs[doc];
        
        public Guid GetPaneId(LayoutDocumentPane pane)
        {
            if (_panes.TryGetValue(pane, out var id))
                return id; // без create вообще

            throw new InvalidOperationException();
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
    }
}
