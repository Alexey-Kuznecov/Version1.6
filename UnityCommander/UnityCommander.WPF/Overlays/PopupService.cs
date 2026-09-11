
using System.Windows;
using System.Windows.Controls.Primitives;
using UnityCommander.WPF.Controls;

namespace UnityCommander.WPF.Overlays
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
            string title,
            FrameworkElement owner,
            object viewModel,
            PopupPlacement placement = PopupPlacement.Top,
            bool isPinnable = false)
        {
            var view = _factory.Create(viewModel);

            Show(title, owner, view, placement, isPinnable);
        }
       
        public void Show<TViewModel>(
            string title,
            FrameworkElement owner,
            PopupPlacement placement = PopupPlacement.Top,
            bool isPinnable = false)
        {
            var view = _factory.Create<TViewModel>();

            Show(title, owner, view, placement, isPinnable);
        }

        private void Show(
            string title,
            FrameworkElement owner,
            FrameworkElement view,
            PopupPlacement placement, 
            bool isPinnable)
        {
            Popup? popup = null;

            var chrome = new PopupChrome();

            var chromeVm = new PopupChromeViewModel(
                content: view,
                title: title,
                isPinnable: isPinnable,
                close: () =>
                {
                    popup?.SetCurrentValue(
                        Popup.IsOpenProperty,
                        false);
                },
                setPinned: pinned =>
                {
                    if (popup is not null)
                        popup.StaysOpen = pinned;
                });

            chrome.DataContext = chromeVm;

            popup = new Popup
            {
                AllowsTransparency = true,
                Child = chrome,
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

            _popup = popup;
            popup.IsOpen = true;
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
