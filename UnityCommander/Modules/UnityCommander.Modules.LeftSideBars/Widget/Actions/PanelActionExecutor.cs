
using System;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Modules.LeftSideBars.Widget.Actions
{
    public sealed class PanelActionExecutor
    {
        private readonly IPanelRegistry _panels;
        private readonly INavigationRegistry _navigationRegistry;

        public PanelActionExecutor(
            IPanelRegistry panels,
            INavigationRegistry navigationRegistry)
        {
            _panels = panels;
            _navigationRegistry = navigationRegistry;
        }

        public void Navigate(
            string path)
        {
            var panel = _panels.GetActivePanel();

            if (panel?.ActiveTabId is not Guid tabId)
                return;

            var navigation = _navigationRegistry.Get(tabId);

            navigation.TryNavigateTo(path);
        }
    }
}
