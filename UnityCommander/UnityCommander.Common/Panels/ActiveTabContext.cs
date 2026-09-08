
using System;
using UnityCommander.Abstractions.Panels;

namespace UnityCommander.Common.Panels
{
    public sealed class ActiveTabContext
    {
        private readonly ITabStateRegistry _states;
        private readonly IPanelRegistry _panelRegistry;

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

        private void OnActiveTabChanged(ActiveTabChangedEvent tab)
        {
            ActiveTabId = tab.TabId;
        }
    }
}
