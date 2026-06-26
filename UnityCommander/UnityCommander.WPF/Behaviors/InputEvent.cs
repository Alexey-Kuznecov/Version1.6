
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.WPF.Behaviors
{
    public readonly struct InputEvent
    {
        public ShortcutKey Key { get; init; }
        public ShortcutModifiers Modifiers { get; init; }
    }
}
