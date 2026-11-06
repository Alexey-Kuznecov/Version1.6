
using AvalonDock.Controls;
using AvalonDock.Layout;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Abstractions;
using UnityCommander.Notifications.Abstraction.Contracts;
using UnityCommander.Services.Interfaces;
using UnityCommander.Services.Interfaces.Docking;

namespace UnityCommander.Services.Docking
{
    public sealed class ToolDockingManager : IToolDockingManager
    {
        private readonly DockingContext _context;

        private readonly IToolCatalogViewFactory _catalogFactory;
        
        private readonly INotificationService _notificationService;

        public ToolDockingManager(
            DockingContext context,
            IToolCatalogViewFactory catalogFactory,
            INotificationService notificationService)
        {
            _context = context;
            _catalogFactory = catalogFactory;
            _notificationService = notificationService;

            _context.ToolManager.AnchorableClosed += ToolManager_AnchorableClosed;
            _context.ToolManager.AnchorableClosing += ToolManager_AnchorableClosing;
            _context.ToolManager.AnchorableHidden += ToolManager_AnchorableHidden;
            _context.ToolManager.AnchorableHiding += ToolManager_AnchorableHiding;
            _context.ToolManager.PreviewMouseDown += ToolManager_PreviewMouseDown;

            //_context.ToolManager.AnchorableContextMenu.Style
        }

        private void ToolManager_PreviewMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not DependencyObject source)
                return;

            var button = FindVisualParent<Button>(source);

            if (button == null)
                return;

            switch (button.Name)
            {
                case "PART_AddTool":
                    e.Handled = true;

                    var title = FindVisualParent<AnchorablePaneTitle>(button);

                    if (title?.DataContext is not LayoutAnchorable anchorable)
                        return;

                    if (anchorable.Parent is not LayoutAnchorablePane pane)
                        return;

                    OpenCatalog(pane);

                    break;

                case "PART_AutoHidePin":
                    e.Handled = true;
                    //OnAutoHideTool();
                    break;

                case "PART_HidePin":
                    e.Handled = true;
                    //OnHideTool();
                    break;
            }
        }

        private void ToolManager_AnchorableHiding(
            object sender, 
            AvalonDock.AnchorableHidingEventArgs e)
        {
        }

        private void ToolManager_AnchorableHidden(
            object sender, 
            AvalonDock.AnchorableHiddenEventArgs e)
        {
        }

        private void ToolManager_AnchorableClosing(
            object sender,
            AvalonDock.AnchorableClosingEventArgs e)
        {
        }

        private void ToolManager_AnchorableClosed(
            object sender, 
            AvalonDock.AnchorableClosedEventArgs e)
        {
        }

        public void OpenCatalog(LayoutAnchorablePane targetPane)
        {
            var catalogView = _catalogFactory.Create(
                 descriptor =>
                 {
                     Create(descriptor, targetPane);
                     CloseCatalog();
                 },
                 CloseCatalog);

            var tool = new LayoutAnchorable
            {
                ContentId = "Tools",
                Title = "Tools",
                CanClose = false,
                CanHide = true,
                Content = catalogView
            };

            targetPane.Children.Add(tool);

            tool.IsSelected = true;
            tool.IsActive = true;
        }

        private void CloseCatalog()
        {
            Remove("Tools");
        }

        public void Create(IToolDescriptor descriptor)
        {
            var pane = GetPane(descriptor.DockSide);

            Create(descriptor, pane);
        }

        public void Create(
            IToolDescriptor descriptor,
            LayoutAnchorablePane targetPane)
        {
            var existing = Find(descriptor.Id);

            if (descriptor.MaxInstances == 1 && existing != null)
            {
                existing.Show();
                existing.IsSelected = true;
                existing.IsActive = true;

                return;
            }

            var instanceCount = GetInstanceCount(descriptor.Id);

            if (instanceCount >= descriptor.MaxInstances)
            {
                _notificationService.ShowWarning(
                    "Tool limit reached",
                    $"You cannot create more than {descriptor.MaxInstances} instances of {descriptor.Title}.");

                return;
            }

            var contentId = descriptor.MaxInstances > 1
                ? $"{descriptor.Id}:{Guid.NewGuid():N}"
                : descriptor.Id;

            var tool = new LayoutAnchorable
            {
                ContentId = contentId,
                Title = descriptor.Title,
                CanClose = false,
                CanHide = true,
                Content = descriptor.Create()
            };

            targetPane.Children.Add(tool);

            tool.IsSelected = true;
            tool.IsActive = true;
        }

        //public void Create(
        //    IToolDescriptor descriptor,
        //    LayoutAnchorablePane targetPane)
        //{
        //    var existing = Find(descriptor.Id);

        //    if (!descriptor.CanCreateMultiple && existing != null)
        //    {
        //        existing.Show();
        //        existing.IsSelected = true;
        //        existing.IsActive = true;

        //        return;
        //    }

        //    var contentId = descriptor.CanCreateMultiple
        //        ? $"{descriptor.Id}:{Guid.NewGuid():N}"
        //        : descriptor.Id;

        //    var tool = new LayoutAnchorable
        //    {
        //        ContentId = contentId,
        //        Title = descriptor.Title,
        //        CanClose = false,
        //        CanHide = true,
        //        Content = descriptor.Create()
        //    };

        //    targetPane.Children.Add(tool);

        //    tool.IsSelected = true;
        //    tool.IsActive = true;
        //}

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

        private int GetInstanceCount(string toolId)
        {
            var layout = _context.ToolManager.Layout;

            var tools = layout.Descendents()
                .OfType<LayoutAnchorable>()
                .Concat(layout.Hidden)
                .Distinct();

            return tools.Count(tool =>
                string.Equals(
                    tool.ContentId,
                    toolId,
                    StringComparison.OrdinalIgnoreCase)
                ||
                tool.ContentId.StartsWith(
                    $"{toolId}:",
                    StringComparison.OrdinalIgnoreCase));
        }

        private static T? FindVisualParent<T>(
            DependencyObject element)
            where T : DependencyObject
        {
            while (element != null)
            {
                if (element is T result)
                    return result;

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
