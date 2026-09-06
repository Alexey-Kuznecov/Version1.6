
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using UnityCommander.Common.Models.Directory;
using UnityCommander.Controls.Layout;
using UnityCommander.Modules.FilePanel.States;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Resolvers
{
    public sealed class FolderDropTargetResolver
      : IDropTargetResolver
    {
        public bool CanResolve(DragDropContext context)
        {
            Debug.WriteLine(
                $"[FolderDropTargetResolver] TargetItem: {context.Target?.GetType().FullName ?? "null"}");

            return context.Target is FolderModel;
        }

        public DropTargetInfo? Resolve(
            DragDropContext context)
        {
            if (context.Target is not FolderModel folder)
                return null;

            if (context.TargetContext is not ContentNode node)
                return null;

            if (node.Context is not FolderNodeContext folderContext)
                return null;

            return new DropTargetInfo
            {
                Path = folder.Path,
                CanNavigate = true,
                NavigateCommand = folderContext.NavigateCommand,
                NavigationTarget = folder
            };
        }
    }
}
