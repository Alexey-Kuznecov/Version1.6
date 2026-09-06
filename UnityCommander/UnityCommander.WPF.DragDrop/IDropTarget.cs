
namespace UnityCommander.WPF.DragDrop
{
    public interface IDropTarget
    {
        void DragOver(IDropInfo dropInfo);

        void DragLeave(IDropInfo dropInfo);

        void DragEnter(IDropInfo dropInfo);

        void Drop(IDropInfo dropInfo);
    }
}