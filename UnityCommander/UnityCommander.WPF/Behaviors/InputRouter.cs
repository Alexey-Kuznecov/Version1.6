
using System.Windows;
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public sealed class InputRouter : IInputRouter
    {
        private readonly IInputService _inputService;
        private readonly IInputCaptureManager _captureManager;
        private readonly IInputContextService _inputContext;

        public InputRouter(
            IInputService inputService,
            IInputCaptureManager captureManager, 
            IInputContextService inputContext)
        {
            _inputService = inputService;
            _captureManager = captureManager;
            _inputContext = inputContext;
        }

        public void Process(Window source, KeyEventArgs e)
        {
            if (source != _inputContext.ActiveWindow)
                return;

            var (key, mod) = WpfShortcutConverter.FromKeyGesture(e, Keyboard.Modifiers);

            var input = new InputEvent
            {
                Key = key,
                Modifiers = mod
            };

            if (_captureManager.TryHandle(input))
            {
                e.Handled = true;
                return;
            }

            if (_inputService.Process(input))
            {
                e.Handled = true;
            }
        }
    }
}
