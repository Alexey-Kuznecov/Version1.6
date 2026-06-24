
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
            var (key, mod) = WpfShortcutConverter.FromKeyGesture(input.Key, input.Modifiers);

            if (!ShortcutKeyValidator.IsValid(input.Key))
                return true;

            _onCaptured(new ShortcutOverride
            {
                CommandId = "",
                Key = key,
                Modifiers = mod
            });

            _exit(); // pop
            return true;
        }
    }
}
