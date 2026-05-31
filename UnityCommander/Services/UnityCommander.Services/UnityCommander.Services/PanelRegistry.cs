
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Panels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class PanelRegistry : IPanelRegistry
    {
        private readonly Dictionary<Guid, IPanel> _panels = new();
        
        private readonly object _lock = new();

        private Guid? _activePanelId;

        public event Action<TabAddedEvent> TabAdded;

        public event Action<TabActionEvent> TabRemoved;

        public Guid? ActivePanelId 
        {
            get => _activePanelId; 
        }

        public IPanel GetPanel(Guid panelId)
        {
            lock (_lock)
            {
                if (!_panels.TryGetValue(panelId, out var panel))
                    throw new InvalidOperationException($"Panel '{panelId}' not found");

                return panel;
            }
        }

        public IReadOnlyList<IPanel> GetAllPanels()
        {
            lock (_lock)
            {
                return _panels.Values.ToList();
            }
        }

        public IPanel GetActivePanel()
        {
            lock (_lock)
            {
                if (_activePanelId == null)
                    throw new InvalidOperationException("Active panel is not set");

                return GetPanel(_activePanelId.Value);
            }
        }

        public void RegisterPanel(Guid panelId)
        {
            lock (_lock)
            {
                if (_panels.ContainsKey(panelId))
                    return;

                _panels[panelId] = new Panel(panelId);

                if (_activePanelId == null)
                    _activePanelId = panelId;
            }
        }

        public void UnregisterPanel(Guid panelId)
        {
            lock (_lock)
            {
                if (_panels.Remove(panelId))
                {
                    if (_activePanelId == panelId)
                        _activePanelId = _panels.Keys.FirstOrDefault();
                }
            }
        }

        public void SetActivePanel(Guid panelId)
        {
            lock (_lock)
            {
                if (!_panels.ContainsKey(panelId))
                    throw new InvalidOperationException($"Panel '{panelId}' not found");

                _activePanelId = panelId;
            }
        }

        public bool SetActiveTab(Guid panelId, Guid tabId)
        {
            lock (_lock)
            {
                var panel = GetPanel(panelId);

                if (!panel.Tabs.Contains(tabId))
                    return false;

                panel.TrySetActiveTab(tabId);
                return true;
            }
        }

        public Guid? GetActivePanelId()
        {
            lock (_lock)
            {
               return _activePanelId;
            }
        }

        // --- Работа с вкладками через панель ---
        public void MoveTab(Guid panelId, Guid tabId)
        {
            lock (_lock)
            {
                IPanel sourcePanel = null;

                foreach (var panel in _panels.Values)
                {
                    if (panel.Tabs.Contains(tabId))
                    {
                        sourcePanel = panel;
                        break;
                    }
                }

                if (sourcePanel == null)
                    return;

                if (sourcePanel.PanelId == panelId)
                    return;

                sourcePanel.RemoveTab(tabId);

                var targetPanel = GetPanel(panelId);
                targetPanel.AddTab(tabId);
            }
        }

        public void AddTab(Guid panelId, Guid tabId)
        {
            lock (_lock)
            {
                GetPanel(panelId).AddTab(tabId);
                TabAdded(new TabAddedEvent() 
                { 
                    PanelId = panelId,
                    TabId = tabId 
                });
            }
        }

        public void RemoveTab(Guid tabId)
        {
            lock (_lock)
            {
                foreach (var panel in _panels.Values)
                {
                    if (panel.Tabs.Contains(tabId))
                    {
                        panel.RemoveTab(tabId);

                        TabRemoved(new TabActionEvent()
                        {
                            PanelId = panel.PanelId,
                            TabId = tabId
                        });
                        return;
                    }
                }
            }
        }

        public Guid? FindPanelByTab(Guid tabId)
        {
            lock (_lock)
            {
                foreach (var panel in _panels.Values)
                {
                    if (panel.Tabs.Contains(tabId))
                        return panel.PanelId;
                }

                return null;
            }
        }

        public IReadOnlyList<Guid> GetTabs(Guid panelId)
        {
            lock (_lock)
            {
                return GetPanel(panelId).Tabs;
            }
        }

        public void EnsurePanel(Guid panelId)
        {
            if (!_panels.ContainsKey(panelId))
            {
                _panels[panelId] = new Panel(panelId);
            }
        }
    }
}
