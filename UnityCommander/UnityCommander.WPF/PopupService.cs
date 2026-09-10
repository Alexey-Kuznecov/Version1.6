
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
            object viewModel,
            PopupPlacement placement = PopupPlacement.Top)
        {
            var view = _factory.Create(viewModel);

            Show(owner, view, placement);
        }

        public void Show<TViewModel>(
            FrameworkElement owner,
            PopupPlacement placement = PopupPlacement.Top)
        {
            var view = _factory.Create<TViewModel>();

            Show(owner, view, placement);
        }

        private void Show(
            FrameworkElement owner,
            FrameworkElement view,
            PopupPlacement placement)
        {
            _popup = new Popup
            {
                Child = view,
                PlacementTarget = owner,
                Placement = PlacementMode.Custom,
                CustomPopupPlacementCallback =
                    (popupSize, targetSize, offset) =>
                        PlacePopup(
                            owner,
                            popupSize,
                            targetSize,
                            placement),
                StaysOpen = false
            };

            _popup.IsOpen = true;
        }

        private CustomPopupPlacement[] PlacePopup(
            FrameworkElement owner,
            Size popupSize,
            Size targetSize,
            PopupPlacement placement)
        {
            var point = placement switch
            {
                PopupPlacement.Top =>
                    new Point(0, -popupSize.Height),

                PopupPlacement.Bottom =>
                    new Point(0, targetSize.Height),

                PopupPlacement.Left =>
                    new Point(-popupSize.Width, 0),

                PopupPlacement.Right =>
                    new Point(targetSize.Width, 0),

                _ => new Point(0, -popupSize.Height)
            };

            return
            [
                new CustomPopupPlacement(
                point,
                PopupPrimaryAxis.None)
            ];
        }

        public void Close()
        {
            _popup?.SetCurrentValue(Popup.IsOpenProperty, false);
            _popup = null;
        }
    }
}
