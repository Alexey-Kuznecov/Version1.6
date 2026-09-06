
using UnityCommander.Common.Models.Directory;
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD.Resolvers
{
    public sealed class NodeDragDropContextResolver
     : IDropContextResolver
    {
        public bool CanResolve(DragDropContext context)
        {
            return context.Target is BaseDirectory;
        }

        public IDropContext Resolve(DragDropContext context)
        {
            return new FilePanelDragDropContext();
        }
    }
}
