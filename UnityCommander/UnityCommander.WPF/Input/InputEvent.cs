using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Input
{
    public readonly struct InputEvent
    {
        public ShortcutKey Key { get; init; }
        public ShortcutModifiers Modifiers { get; init; }
    }
}
