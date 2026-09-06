
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Resolvers
{
    public sealed class DirectoryDropTargetResolver
       : IDropTargetResolver
    {
        public bool CanResolve(DragDropContext context)
        {
            Debug.WriteLine(
                $"[DirectoryDropTargetResolver] TargetItem: {context.Target?.GetType().FullName ?? "null"}");

            return context.VisualTarget is ListView;
        }

        public DropTargetInfo? Resolve(DragDropContext context)
        {
            if (context.VisualTarget is not FrameworkElement element)
                return null;

            if (element.DataContext is not ContentNode node)
                return null;

            if (node.Context is not BaseNodeContext nodeContext)
                return null;

            var result = new DropTargetInfo
            {
                Path = nodeContext.Current,
            };

            if (nodeContext is FolderNodeContext folderContext)
            {
                result.CanNavigate = true;
                result.NavigateCommand = folderContext.NavigateCommand;
                result.NavigationTarget = folderContext.SelectedFolder;
                // Здесь уже конкретная navigation target,
                // если она действительно нужна.
            }

            return result;
        }
    }
}
