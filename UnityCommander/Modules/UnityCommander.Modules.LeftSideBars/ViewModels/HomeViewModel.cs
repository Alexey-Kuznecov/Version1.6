
using System.Collections.ObjectModel;
using System.Windows;
using UnityCommander.WPF.Widget;

namespace UnityCommander.Modules.LeftSideBars.ViewModels
{
    public sealed class HomeViewModel
    {
        public ObservableCollection<FrameworkElement> Widgets { get; }

        public HomeViewModel(IWidgetFactory widgetFactory)
        {
            Widgets =
            [
                widgetFactory.Create("drives")
            ];
        }
    }
}
