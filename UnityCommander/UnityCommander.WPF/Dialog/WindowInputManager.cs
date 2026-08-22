
using System.Windows;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.WPF.Input;

namespace UnityCommander.WPF.Dialog
{
    public sealed class WindowInputManager
    {
        private readonly IInputRouter _router;
        private readonly IInputContextService _context;

        public WindowInputManager(
            IInputRouter router,
            IInputContextService context)
        {
            _router = router;
            _context = context;
        }

        public void Attach(
            Window window,
            ShortcutScope scope)
        {
            window.PreviewKeyDown += OnPreviewKeyDown;
            window.PreviewKeyUp += OnPreviewKeyUp;
            window.Closed += OnClosed;

            _context.Attach(window, scope);
        }

        public void Detach(Window window)
        {
            window.PreviewKeyDown -= OnPreviewKeyDown;
            window.PreviewKeyUp -= OnPreviewKeyUp;
            window.Closed -= OnClosed;

            _context.Detach(window);
        }

        private void OnPreviewKeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (sender is not Window window)
                return;

            _context.SetActive(window);

            _router.Process(window, e);
        }

        private void OnPreviewKeyUp(
            object sender,
            KeyEventArgs e)
        {
            if (sender is not Window window)
                return;

            _router.Process(window, e);
        }

        private void OnClosed(
            object sender,
            EventArgs e)
        {
            if (sender is Window window)
                Detach(window);
        }
    }
}
