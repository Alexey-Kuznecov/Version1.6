
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public readonly struct InputEvent
    {
        public Key Key { get; init; }
        public ModifierKeys Modifiers { get; init; }
    }
}
