
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
            return context.VisualTarget is ListView;
        }

        public DropTargetInfo? Resolve(
            DragDropContext context)
        {
            if (context.VisualTarget is not FrameworkElement element)
                return null;

            if (element.DataContext is not ContentNode node)
                return null;

            if (node.Context is not BaseNodeContext nodeContext)
                return null;

            return new DropTargetInfo
            {
                Path = nodeContext.Current
            };
        }
    }
}
