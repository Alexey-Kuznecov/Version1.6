
using System.Windows.Controls;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Resolvers
{
    public sealed class NavigationButtonDropTargetResolver
     : IDropTargetResolver
    {
        public bool CanResolve(DragDropContext context)
        {
            return context.VisualTarget is Button button
                && NavigationButtonDragDrop.GetEnable(button);
        }

        public DropTargetInfo? Resolve(
            DragDropContext context)
        {
            if (context.VisualTarget is not Button button)
                return null;

            var path =
                NavigationButtonDragDrop.GetDropPath(button);

            if (string.IsNullOrWhiteSpace(path))
                return null;

            return new DropTargetInfo
            {
                Path = path,
                CanNavigate = button.Command is not null,
                NavigateCommand = button.Command,
                NavigationTarget = path
            };
        }
    }
}
