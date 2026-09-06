
using System.Windows;

namespace UnityCommander.WPF
{
    public interface ICursorTargetService
    {
        CursorTarget? GetCurrent(UIElement source);

        void Update(
            UIElement source,
            Point position);

        void Clear(UIElement source);
    }
}
