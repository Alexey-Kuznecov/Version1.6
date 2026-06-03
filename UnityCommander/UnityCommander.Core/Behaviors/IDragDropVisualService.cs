
using System.Windows;

namespace UnityCommander.Core.Behaviors
{
    public interface IDragDropVisualService
    {
        void Apply(UIElement target, DragDropResult result);
        void Clear(UIElement target);
    }
}
