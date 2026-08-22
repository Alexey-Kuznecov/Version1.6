
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Input
{
    public sealed class InputState : IInputState
    {
        private readonly HashSet<ShortcutKey> _pressed = new();

        public event EventHandler<InputEvent>? KeyPressed;

        public event EventHandler<InputEvent>? KeyReleased;

        public bool IsDown(ShortcutKey key)
            => _pressed.Contains(key);

        public void UpdateKeyDown(InputEvent input)
        {
            if (!_pressed.Add(input.Key))
                return;

            KeyPressed?.Invoke(this, input);
        }

        public void UpdateKeyUp(InputEvent input)
        {
            if (!_pressed.Remove(input.Key))
                return;

            KeyReleased?.Invoke(this, input);
        }

        public void Clear()
        {
            _pressed.Clear();
        }
    }
}
