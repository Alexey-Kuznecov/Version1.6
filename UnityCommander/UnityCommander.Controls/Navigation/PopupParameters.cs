
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.Controls.Navigation
{
    internal sealed class PopupParameters
    {
        public UIElement Anchor { get; internal set; }
        public NavigationPathItem CurrentItem { get; init; }
        public ICommand NavigateCommand { get; init; }
    }
}
