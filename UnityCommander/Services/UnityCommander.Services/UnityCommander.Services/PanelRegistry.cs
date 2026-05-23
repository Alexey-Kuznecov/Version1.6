using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class PanelRegistry : IPanelRegistry
    {
        private readonly Dictionary<Guid, IPanel> _panels = new();
        
        private readonly object _lock = new();

        private Guid? _activePanelId;

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

        public Guid? GetActivePanelId()
        {
            lock (_lock)
            {
               return _activePanelId;
            }
        }

        // --- Работа с вкладками через панель ---

        public void AddTab(Guid panelId, Guid tabId)
        {
            lock (_lock)
            {
                GetPanel(panelId).AddTab(tabId);
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
                        return;
                    }
                }
            }
        }

        public void SetActiveTab(Guid panelId, Guid tabId)
        {
            lock (_lock)
            {
                GetPanel(panelId).SetActiveTab(tabId);
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
