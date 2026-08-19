
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public interface IDropTargetResolver
    {
        bool CanResolve(DragDropContext context);

        DropTargetInfo Resolve(DragDropContext context);
    }
}
