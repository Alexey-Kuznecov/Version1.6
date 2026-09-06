
using System.Windows;

namespace UnityCommander.UI.Visual
{
    public interface IVisualElementRegistry
    {
        void Register(object model, FrameworkElement element);
        void Unregister(object model, FrameworkElement element);

        FrameworkElement? GetElement(object model);
    }
}
