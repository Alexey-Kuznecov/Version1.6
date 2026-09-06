
namespace UnityCommander.WPF.DragDrop
{
    public interface IDragDropContextFactory
    {
        public DragDropContext Create(
            IDropInfo info);
    }
}
