
using System.Collections.ObjectModel;
using UnityCommander.WPF.Widget;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public sealed class HomeViewModel
    {
        public ObservableCollection<WidgetItem> Widgets { get; } = [];

        public HomeViewModel(IWidgetFactory widgetFactory)
        {
            Widgets =
            [
                widgetFactory.Create("drives", 0),
                widgetFactory.Create("system-folders", 1),
                widgetFactory.Create("history", 2),
                widgetFactory.Create("favorites", 3)
            ];
        }
    }
}
