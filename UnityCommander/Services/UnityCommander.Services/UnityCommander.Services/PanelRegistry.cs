using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Common.Debug;
using UnityCommander.Common.Diagnostic;
using UnityCommander.Common.Panels;
using UnityCommander.Services.Interfaces;

#nullable enable

namespace UnityCommander.Services
{
    public class PanelRegistry :  IPanelRegistry, IDebuggable<DebugPanelState>, IDiagnosticSource
    {
        private readonly Dictionary<Guid, IPanel> _panels = new();
        
        private readonly object _lock = new();

        private Guid? _activePanelId;

        public event Action<TabAddedEvent>? TabAdded;

        public event Action<TabActionEvent>? TabRemoved;

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
            Guid active;
            lock (_lock)
            {
                if (_activePanelId == null)
                    throw new InvalidOperationException("Active panel is not set");

                active = _activePanelId.Value;
            }

            return GetPanel(active);
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

                if (TabAdded == null)
                    return;

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

                        if (TabRemoved == null)
                            continue;

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

        public void RemovePanel(Guid panelId)
        {
            _panels.Remove(panelId);
        }

        public bool IsEmpty(Guid panelId)
        {
            if (_panels.TryGetValue(panelId, out var result))
                return !result.Tabs.Any();

            return true;
        }

        public bool Contains(Guid tabId)
            => _panels.Values.Any(panel 
                => panel.Tabs.Contains(tabId));


        #region DEBUG AND DIAGNOSTICS

        public string Name => "panel";

        public IReadOnlyDictionary<string, object?> GetState()
        {
            lock (_lock)
            {
                return new Dictionary<string, object?>
                {
                    ["PanelCount"] = _panels.Count,
                    ["TabCount"] = _panels.Values.Sum(panel => panel.Tabs.Count),
                    ["ActivePanel"] = _activePanelId ?? Guid.Empty,
                };
            }
        }

        public DebugPanelState GetDebugState()
        {
            lock (_lock)
            {
                return new DebugPanelState()
                {
                    PanelCount = _panels.Count,
                    TabCount = _panels.Values.Sum(panel => panel.Tabs.Count),
                    ActivePanel = _activePanelId ?? Guid.Empty,
                };
            }
        }

        public string Describe()
        {
            return new string(
                "Panel registry diagnostics.\n" +
                "PanelCount  - number of panels\n" +
                "TabCount    - total tab count\n" +
                "ActivePanel   - current active panel");
        }

        #endregion
    }
}
