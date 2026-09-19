
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UnityCommander.Abstractions.Panels;
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
            //Debug.WriteLine(
            //    $"[DirectoryDropTargetResolver] TargetItem: {context.Target?.GetType().FullName ?? "null"}");

            return context.VisualTarget is ListView;
        }

        public DropTargetInfo? Resolve(DragDropContext context)
        {
            if (context.VisualTarget is not ListView listView)
                return null;

            if (listView.DataContext is not ContentNode node)
                return null;

            if (node.Context is UnifiedNodeContext unified)
            {
                var position = Mouse.GetPosition(listView);
                var hit = listView.InputHitTest(position) as DependencyObject;
                var container = FindParent<ListViewItem>(hit);

                if (container?.DataContext is IFolderItem folder)
                {
                    return new DropTargetInfo
                    {
                        Path = folder.Path,
                        CanNavigate = true,
                        NavigationTarget = folder
                    };
                }

                if (container?.DataContext is IFileItem)
                {
                    return new DropTargetInfo
                    {
                        Path = unified.Folder.Current,
                        CanNavigate = false
                    };
                }

                return new DropTargetInfo
                {
                    Path = unified.Folder.Current,
                    CanNavigate = false
                };
            }


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
            }

            return result;
        }

        private static T? FindParent<T>(DependencyObject? element)
            where T : DependencyObject
        {
            while (element is not null)
            {
                if (element is T result)
                    return result;

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }
    }
}
