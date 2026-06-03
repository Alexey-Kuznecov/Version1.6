using System.Windows;

namespace UnityCommander.Core.DragDrop
{
    /// <summary>
    /// Interface implemented by Drop Handlers.
    /// </summary>
    public interface IDropTarget
    {
        void DragOver(IDropInfo dropInfo);

        void DragLeave(IDropInfo dropInfo);

        void DragEnter(IDropInfo dropInfo);

        void Drop(IDropInfo dropInfo);
    }
}