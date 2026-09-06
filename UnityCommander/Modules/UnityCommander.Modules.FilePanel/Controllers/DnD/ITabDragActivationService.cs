
using UnityCommander.WPF.DragDrop;

namespace UnityCommander.Modules.FilePanel.Controllers.DnD
{
    public interface ITabDragActivationService
    {
        void DragOver(DragDropContext context);
        void DragLeave();
    }
}
