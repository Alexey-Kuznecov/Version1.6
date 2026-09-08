
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
                new WidgetItem
                {
                    Id = "drives",
                    Title = "Drives",
                    View = widgetFactory.Create("drives"),
                    Order = 0
                },

                new WidgetItem
                {
                    Id = "system-folders",
                    Title = "System Folders",
                    View = widgetFactory.Create("system-folders"),
                    Order = 1
                }
            ];
        }
    }
}
