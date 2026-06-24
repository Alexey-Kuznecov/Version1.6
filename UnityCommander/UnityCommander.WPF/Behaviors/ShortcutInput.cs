
using System.Windows.Input;

namespace UnityCommander.WPF.Behaviors
{
    public readonly record struct ShortcutInput(
       Key Key,
       ModifierKeys Modifiers);
}
