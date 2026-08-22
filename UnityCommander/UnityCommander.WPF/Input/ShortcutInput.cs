
using System.Windows.Input;

namespace UnityCommander.WPF.Input
{
    public readonly record struct ShortcutInput(
       Key Key,
       ModifierKeys Modifiers);
}
