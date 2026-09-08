
using System.Windows;

namespace UnityCommander.WPF.Widget
{
    public interface IWidgetFactory
    {
        FrameworkElement Create(string widgetId);
    }
}
