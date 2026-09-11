
using System.Windows;

namespace UnityCommander.WPF.Overlays
{
    public interface IPopupService
    {
        public void Show<TViewModel>(
            string title,
            FrameworkElement owner,
            PopupPlacement placement = PopupPlacement.Top,
            bool isPinnable = false);

        public void Show(
            string title,
            FrameworkElement owner,
            object viewModel,
            PopupPlacement placement = PopupPlacement.Top,
            bool isPinnable = false);
       
         void Close();
    }
}
