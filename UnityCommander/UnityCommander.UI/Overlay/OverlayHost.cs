using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnityCommander.UI.Overlay
{
    public sealed class OverlayHost : ContentControl
    {
        public OverlayHost()
        {
            MouseDown += OnMouseDown; 
            IsHitTestVisible = true;
        }

        public void Show(FrameworkElement overlay)
        {
            overlay.IsHitTestVisible = true;

            Content = overlay;
        }

        public void Hide()
        {
            Content = null;
        }

        private void OnMouseDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (e.OriginalSource == this)
            {
                Hide();
            }
        }
    }
}