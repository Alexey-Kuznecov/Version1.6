
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Input
{
    public interface IInputState
    {
        bool IsDown(ShortcutKey key);

        void UpdateKeyDown(InputEvent input);

        void UpdateKeyUp(InputEvent input);

        event EventHandler<InputEvent>? KeyPressed;

        event EventHandler<InputEvent>? KeyReleased;
    }
}
