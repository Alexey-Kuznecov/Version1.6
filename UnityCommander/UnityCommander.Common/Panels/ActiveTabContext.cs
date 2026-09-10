
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Common.Panels
{
    public sealed class ActiveTabContext
    {
        private readonly ITabStateRegistry _states;
        private readonly IPanelRegistry _panelRegistry;

        private readonly List<Action<string>> _pathChangedHandlers = [];

        public Guid ActiveTabId { get; private set; }

        public TabState? Active =>
            _states.Get(ActiveTabId);

        public string? CurrentPath =>
            Active?.CurrentPath;

        public ActiveTabContext(
            ITabStateRegistry states,
            IPanelRegistry panelRegistry)
        {
            _states = states;
            _panelRegistry = panelRegistry;

            _panelRegistry.ActiveTabChanged += OnActiveTabChanged;
        }

        public void SubscribeCurrentPath(Action<string> handler)
        {
            _pathChangedHandlers.Add(handler);

            Active?.CurrentPathChanged += handler;
        }

        public void UnsubscribeCurrentPath(Action<string> handler)
        {
            _pathChangedHandlers.Remove(handler);

            Active?.CurrentPathChanged -= handler;
        }

        private void OnActiveTabChanged(ActiveTabChangedEvent tab)
        {
            var previous = Active;

            if (previous is not null)
            {
                foreach (var handler in _pathChangedHandlers)
                    previous.CurrentPathChanged -= handler;
            }

            ActiveTabId = tab.TabId;

            var current = Active;

            if (current is not null)
            {
                foreach (var handler in _pathChangedHandlers)
                    current.CurrentPathChanged += handler;
            }
        }
    }
}
