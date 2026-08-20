
namespace UnityCommander.WPF.DragDrop
{
    public interface IDragDropController
    {
        public DragDropResult DragOver(
            DragDropContext context);
        
        public Task DropAsync(
            DragDropContext context);
        
        public void DragLeave(
            DragDropContext context);

        public void DragEnter(
            DragDropContext context);
    }
}
