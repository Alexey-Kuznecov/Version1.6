

using System.Windows.Controls;
using UnityCommander.Common.Models.Directory;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public sealed class NodeDragDropContextResolver
      : IDropContextResolver
    {
        public bool CanResolve(DragDropContext context)
        {
            return context.Target is BaseDirectory
                || context.VisualTarget is ListView;
        }

        public IDropContext Resolve(DragDropContext context)
        {
            return new FilePanelDragDropContext();
        }
    }
}
