
using AvalonDock.Layout;
using System;
using System.Linq;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Docking
{
    public sealed class ToolDockingManager : IToolDockingManager
    {
        private readonly DockingContext _context;

        public ToolDockingManager(
            DockingContext context)
        {
            _context = context;

            _context.ToolManager.AnchorableClosed += ToolManager_AnchorableClosed;
            _context.ToolManager.AnchorableClosing += ToolManager_AnchorableClosing;
            _context.ToolManager.AnchorableHidden += ToolManager_AnchorableHidden;
            _context.ToolManager.AnchorableHiding += ToolManager_AnchorableHiding; ;
        }

        private void ToolManager_AnchorableHiding(object sender, AvalonDock.AnchorableHidingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ToolManager_AnchorableHidden(object sender, AvalonDock.AnchorableHiddenEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ToolManager_AnchorableClosing(object sender, AvalonDock.AnchorableClosingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ToolManager_AnchorableClosed(object sender, AvalonDock.AnchorableClosedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void Create(IToolDescriptor descriptor)
        {
            var existing = Find(descriptor.Id);

            if (!descriptor.CanCreateMultiple)
            {
                existing.IsSelected = true;
                existing.IsActive = true;
                return;
            }

            var contentId = $"{descriptor.Id}:{Guid.NewGuid():N}";

            var tool = new LayoutAnchorable
            {
                ContentId = contentId,
                Title = descriptor.Title,
                CanClose = false,
                CanHide = true,
                Content = descriptor.Create()
            };

            var pane = GetPane(descriptor.DockSide);

            pane.Children.Add(tool);

            tool.IsSelected = true;
            tool.IsActive = true;
        }

        public void Remove(string toolId)
        {
            var tool = Find(toolId);

            if (tool == null)
                return;

            var content = tool.Content;

            tool.Content = null;
            tool.Parent?.RemoveChild(tool);

            if (content is IDisposable disposable)
                disposable.Dispose();
        }

        private LayoutAnchorable? Find(string toolId)
        {
            var layout = _context.ToolManager.Layout;

            var tool = layout
                .Descendents()
                .OfType<LayoutAnchorable>()
                .FirstOrDefault(x =>
                    string.Equals(
                        x.ContentId,
                        toolId,
                        StringComparison.OrdinalIgnoreCase));

            if (tool != null)
                return tool;

            return layout.Hidden
                .FirstOrDefault(x =>
                    string.Equals(
                        x.ContentId,
                        toolId,
                        StringComparison.OrdinalIgnoreCase));
        }

        public void Show(string toolId)
        {
            var tool = Find(toolId);

            if (tool == null)
                return;

            tool.Show();
            tool.IsActive = true;
        }

        public void Hide(string toolId)
        {
            var tool = Find(toolId);

            if (tool == null)
                return;

            tool.Hide();
        }

        private LayoutAnchorablePane GetPane(ToolDockSide side)
        {
            var panes = _context.ToolManager.Layout
                .Descendents()
                .OfType<LayoutAnchorablePane>()
                .ToList();

            var index = side switch
            {
                ToolDockSide.Left => 0,
                ToolDockSide.Center => 1,
                ToolDockSide.Right => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(side))
            };

            if (index >= panes.Count)
                throw new InvalidOperationException(
                    $"Tool docking pane '{side}' was not found.");

            return panes[index];
        }
    }
}
