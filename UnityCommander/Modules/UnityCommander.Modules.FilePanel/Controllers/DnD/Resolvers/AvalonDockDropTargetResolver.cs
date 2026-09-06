
using AvalonDock.Controls;
using AvalonDock.Layout;
using System;
using UnityCommander.Abstractions.Panels;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Resolvers
{
    public sealed class AvalonDockDropTargetResolver
     : IDropTargetResolver
    {
        private readonly ITabRegistry _tabs;

        public AvalonDockDropTargetResolver(
            ITabRegistry tabs)
        {
            _tabs = tabs;
        }

        public bool CanResolve(DragDropContext context)
        {
            return context.VisualTarget
                is LayoutDocumentTabItem;
        }

        public DropTargetInfo? Resolve(
            DragDropContext context)
        {
            if (context.VisualTarget
                is not LayoutDocumentTabItem tab)
                return null;

            if (tab.Model
                is not LayoutDocument document)
                return null;

            if (!Guid.TryParse(document.ContentId, out var tabId))
                return null;

            var registeredTab = _tabs.GetTab(tabId);

            if (registeredTab == null)
                return null;

            return new DropTargetInfo
            {
                Path = registeredTab.GetCurrentPath(),
                TabId = registeredTab.TabId
            };
        }
    }
}
