
using System.Windows;

namespace UnityCommander.UI.Overlay
{
    public interface IOverlayService
    {
        public void Show(
            FrameworkElement target,
            FrameworkElement overlay);

        public void Hide(
          FrameworkElement target,
          FrameworkElement overlay);
    }
}
