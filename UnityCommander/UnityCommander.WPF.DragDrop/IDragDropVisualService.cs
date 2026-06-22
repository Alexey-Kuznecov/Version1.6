
using System.Windows;

namespace UnityCommander.WPF.DragDrop
{
    public interface IDragDropVisualService
    {
        void Apply(UIElement target, DragDropResult result);
        void Clear(UIElement target);
    }
}
