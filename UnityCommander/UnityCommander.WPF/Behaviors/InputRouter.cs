
using System.Windows;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public sealed class InputRouter : IInputRouter
    {
        private readonly IInputService _inputService;
        private readonly IInputCaptureManager _captureManager;
        private readonly IShortcutContextService _context;
        private readonly IInputContextService _inputContext;

        public InputRouter(
            IInputService inputService,
            IInputCaptureManager captureManager, 
            IShortcutContextService context, 
            IInputContextService inputContext)
        {
            _inputService = inputService;
            _captureManager = captureManager;
            _context = context;
            _inputContext = inputContext;
        }

        public void Process(Window source, KeyEventArgs e)
        {
            if (source != _inputContext.ActiveWindow)
                return;

            var input = new InputEvent
            {
                Key = e.Key,
                Modifiers = Keyboard.Modifiers
            };

            if (_captureManager.TryHandle(input))
            {
                e.Handled = true;
                return;
            }

            // 2. иначе обычные горячие клавиши
            _inputService.Process(e);
        }
    }
}
