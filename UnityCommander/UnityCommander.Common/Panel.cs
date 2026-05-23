using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityCommander.Common
{
    public class Panel : IPanel
    {
        public Guid PanelId { get; }

        private readonly List<Guid> _tabs = new();

        public IReadOnlyList<Guid> Tabs => _tabs;

        public Guid? ActiveTabId { get; private set; }

        public Panel(Guid panelId)
        {
            PanelId = panelId;
        }

        public void AddTab(Guid tabId)
        {
            if (!_tabs.Contains(tabId))
                _tabs.Add(tabId);

            if (ActiveTabId == null)
                ActiveTabId = tabId;
        }

        public void RemoveTab(Guid tabId)
        {
            _tabs.Remove(tabId);

            if (ActiveTabId == tabId)
                ActiveTabId = _tabs.FirstOrDefault();
        }

        public void SetActiveTab(Guid tabId)
        {
            if (!_tabs.Contains(tabId))
                throw new InvalidOperationException("Tab not in panel");

            ActiveTabId = tabId;
        }
    }
}
