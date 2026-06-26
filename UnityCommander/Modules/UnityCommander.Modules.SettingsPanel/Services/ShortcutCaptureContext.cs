
using System;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class ShortcutCaptureContext : IInputContext
    {
        private readonly Action<InputEvent> _onCaptured;
        private readonly Action _exit;

        public ShortcutCaptureContext(
            Action<InputEvent> onCaptured,
            Action exit)
        {
            _onCaptured = onCaptured;
            _exit = exit;
        }

        public bool Handle(InputEvent input)
        {
            if (input.Key == ShortcutKey.Escape)
            {
                _exit();
                return true;
            }

            if (!ShortcutKeyValidator.IsValid(input.Key, input.Modifiers))
                return true;

            _onCaptured(input);

            _exit();
            return true;
        }
    }
}
