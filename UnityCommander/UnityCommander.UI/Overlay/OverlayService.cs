
using System.Windows;
using System.Windows.Controls;

namespace UnityCommander.UI.Overlay
{
    public sealed class OverlayService : IOverlayService
    {
        private FrameworkElement? _currentTarget;
        private FrameworkElement? _currentOverlay;

        public void Show(
          FrameworkElement target,
          FrameworkElement overlay)
        {
            if (_currentTarget != null &&
                _currentOverlay != null)
            {
                Hide(_currentTarget, _currentOverlay);
            }

            if (target is not Control control)
                return;

            if (control.Template.FindName("OverlayHost", control) is not OverlayHost host)
                return;

            host.Show(overlay);

            _currentTarget = target;
            _currentOverlay = overlay;
        }

        public void Hide(
            FrameworkElement target,
            FrameworkElement overlay)
        {
            if (target is not Control control)
                return;

            if (control.Template.FindName("OverlayHost", control) is OverlayHost host)
            {
                host.Hide();
            }

            if (ReferenceEquals(_currentTarget, target) &&
                ReferenceEquals(_currentOverlay, overlay))
            {
                _currentTarget = null;
                _currentOverlay = null;
            }
        }
    }
}
