

namespace UnityCommander.WPF.DragDrop
{
    public interface IDropContextResolver
    {
        bool CanResolve(DragDropContext context);

        IDropContext Resolve(DragDropContext context);
    }
}
