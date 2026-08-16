
using System.Windows;

namespace UnityCommander.WPF
{
    public interface IViewFactory
    {
        FrameworkElement Create(object viewModel);
    }
}