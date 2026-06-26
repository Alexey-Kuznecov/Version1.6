
using System;
using System.Windows.Input;
using UnityCommander.Abstractions.Keyboard;
using UnityCommander.WPF.Behaviors;

namespace UnityCommander.Modules.SettingsPanel.Services
{
    public class ShortcutCaptureContext : IInputContext
    {
        private readonly Action<ShortcutOverride> _onCaptured;
        private readonly Action _exit;

        public ShortcutCaptureContext(
            Action<ShortcutOverride> onCaptured,
            Action exit)
        {
            _onCaptured = onCaptured;
            _exit = exit;
        }

        public bool Handle(InputEvent input)
        {
            if (input.Key == Key.Escape)
            {
                _exit();
                return true;
            }

            var (key, mod) =
                WpfShortcutConverter.FromKeyGesture(
                    input.Key,
                    input.Modifiers);

            if (input.Modifiers == ModifierKeys.None)
                return false;

            if (!ShortcutKeyValidator.IsValid(input.Key))
                return true;

            _onCaptured(new ShortcutOverride
            {
                Key = key,
                Modifiers = mod
            });

            _exit(); // pop
            return true;
        }
    }
}
