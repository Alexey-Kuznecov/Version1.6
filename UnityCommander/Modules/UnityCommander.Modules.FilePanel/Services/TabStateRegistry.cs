
using System;
using System.Collections.Generic;
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
    }
}
