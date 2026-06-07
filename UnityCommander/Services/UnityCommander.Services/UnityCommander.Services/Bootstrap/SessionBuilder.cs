
using System;
using System.Linq;
using UnityCommander.Common.State;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Bootstrap;

namespace UnityCommander.Services.Bootstrap
{
    public class SessionBuilder : ISessionBuilder
    {
        private readonly IPanelRegistry _panelRegistry;
        private readonly ITabRegistry _tabRegistry;

        public SessionBuilder(
            IPanelRegistry panelRegistry,
            ITabRegistry tabRegistry)
        {
            _panelRegistry = panelRegistry;
            _tabRegistry = tabRegistry;
        }

        public void Build(AppSessionState state)
        {
            var panels = _panelRegistry.GetAllPanels();

            state.Panels.Clear();
            state.Panels = panels.Select(panel =>
            {
                return new PanelSessionState
                {
                    PanelId = panel.PanelId,

                    Tabs = panel.Tabs
                        .Select(tabId =>
                        {
                            var tab = _tabRegistry.GetTab(tabId);

                            return new TabSessionState
                            {
                                Title = tab?.GetCurrentPath(),
                                TabId = tabId,
                                Path = tab?.GetCurrentPath()
                            };
                        })
                        .ToList(),

                    ActiveTabId = panel.ActiveTabId ?? Guid.Empty
                };
            }).ToList();
        }
    }
}
