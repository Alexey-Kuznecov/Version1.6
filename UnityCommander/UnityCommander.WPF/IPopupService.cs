
using System.Windows;

namespace UnityCommander.WPF
{
    public interface IPopupService
    {
        void Show(FrameworkElement owner, object viewModel);

        void Close();
    }
}
