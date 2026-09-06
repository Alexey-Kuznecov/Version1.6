
using System.Windows;

namespace UnityCommander.WPF
{
    public interface IPopupService
    {
        public void Show(
            FrameworkElement owner,
            object viewModel,
            PopupPlacement placement = PopupPlacement.Top);

        public void Show<TViewModel>(
            FrameworkElement owner,
            PopupPlacement placement = PopupPlacement.Top);

        void Close();
    }
}
