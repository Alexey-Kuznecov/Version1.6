

namespace UnityCommander.WPF.Widget
{
    public interface IWidgetFactory
    {
        WidgetItem Create(string widgetId, int order);
    }
}
