
namespace UnityCommander.WPF.DragDrop
{
    public interface IDragDropHandler
    {
        bool CanHandle(IDropContext context);

        DragDropResult DragOver(
            IDropContext dropContext,
            DragDropContext context);

        Task DropAsync(
            IDropContext dropContext,
            DragDropContext context);

        public void DragLeave(
            IDropContext dropContext,
            DragDropContext context);
    }
}
