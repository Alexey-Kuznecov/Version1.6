
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace UnityCommander.Controls.Navigation
{
    public sealed class NavigationPopupService
    {
        public void Show(
            UIElement placementTarget,
            string path,
            ICommand navigateCommand)
        {
            var popup = new NavigationPopup();

            var viewModel =
                new NavigationPopupViewModel(
                    path,
                    navigateCommand);

            popup.DataContext = viewModel;

            var popupWindow = new Popup
            {
                Child = popup,
                PlacementTarget = placementTarget,
                Placement = PlacementMode.Top,
                StaysOpen = false
            };

            popupWindow.SetBinding(
                Popup.IsOpenProperty,
                new Binding(nameof(
                    NavigationPopupViewModel.PopupIsOpen))
                {
                    Mode = BindingMode.TwoWay,
                    Source = viewModel
                });

            popupWindow.IsOpen = true;
        }
    }
}
