
using System;
using System.Collections.Generic;
using System.Linq;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Modules.FilePanel.Services
{
    public sealed class TabStateRegistry : ITabStateRegistry
    {
        private readonly Dictionary<Guid, TabState> _states = new();

        public void Register(TabState state)
            => _states[state.TabId] = state;

        public void Unregister(Guid tabId)
            => _states.Remove(tabId);

        public TabState? Get(Guid tabId)
            => _states.GetValueOrDefault(tabId);

        public ViewMode GetPanelViewMode(Guid panelId)
        {
            return _states.Values
                .FirstOrDefault(x => x.PanelId == panelId)
                ?.ViewMode ?? ViewMode.Table;
        }

        public void SetPanelViewMode(Guid panelId, ViewMode mode)
        {
            foreach (var state in _states.Values)
            {
                if (state.PanelId != panelId)
                    continue;

                state.ViewMode = mode;
            }
        }
    }
}
