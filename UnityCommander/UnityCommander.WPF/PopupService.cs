
using System.Windows;
using System.Windows.Controls.Primitives;

namespace UnityCommander.WPF
{
    public sealed class PopupService : IPopupService
    {
        private readonly IViewFactory _factory;

        private Popup? _popup;

        public PopupService(IViewFactory views)
        {
            _factory = views;
        }

        public void Show(
          FrameworkElement owner,
          object viewModel)
        {
            Point location = owner.PointToScreen(new Point(0, 0));

            var view = _factory.Create(viewModel);

            _popup = new Popup
            {
                Child = view,
                PlacementTarget = owner,
                Placement = PlacementMode.Custom,
                CustomPopupPlacementCallback = PlacePopup,
                StaysOpen = false,
            };

            _popup.IsOpen = true;
        }

        private CustomPopupPlacement[] PlacePopup(
            Size popupSize,
            Size targetSize,
            Point offset)
        {
            var window = Window.GetWindow(_popup.PlacementTarget);

            double x = 0;

            if (window != null)
            {
                // Правая граница Target относительно окна
                Point p = _popup.PlacementTarget.TranslatePoint(
                    new Point(targetSize.Width, 0), window);

                double right = p.X + popupSize.Width;

                const double margin = -10;

                if (right > window.ActualWidth - margin)
                {
                    x -= right - (window.ActualWidth - margin);
                }
            }

            return
            [
                new CustomPopupPlacement(new Point(x, -popupSize.Height),
                    PopupPrimaryAxis.Horizontal)
            ];
        }

        public void Close()
        {
            _popup?.SetCurrentValue(Popup.IsOpenProperty, false);
            _popup = null;
        }
    }
}
