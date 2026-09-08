
using System;
using System.Collections.Generic;
using UnityCommander.Abstractions.Panels;
using UnityCommander.Services.Interfaces;
using UnityCommander.UI.Interaction;

namespace UnityCommander.Modules.LeftSideBars.Widget
{
    public sealed class PanelActionProvider : IContextActionProvider
    {
        private readonly IPanelRegistry _panels;
        private readonly INavigationRegistry _navigationRegistry;

        public PanelActionProvider(
            IPanelRegistry panels, 
            INavigationRegistry navigationRegistry)
        {
            _panels = panels;
            _navigationRegistry = navigationRegistry;
        }

        public IEnumerable<IContextAction>? GetActions(
            IInteractionContext context)
        {
            if (context is not DiskContext disk)
                yield break;

            var count = 0;

            foreach (var panel in _panels.GetAllPanels())
            {
                if (panel.ActiveTabId is not Guid tabId)
                    continue;

                var manager = _navigationRegistry.Get(tabId);

                yield return new OpenInPanelAction(
                    manager,
                    panel.PanelId,
                    (++count).ToString(),
                    disk.Path); 
            }
        }
    }
}
