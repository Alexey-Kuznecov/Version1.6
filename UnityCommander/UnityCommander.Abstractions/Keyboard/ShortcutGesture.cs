
namespace UnityCommander.Abstractions.Keyboard
{
    public readonly record struct ShortcutGesture(
      ShortcutKey Key,
      ShortcutModifiers Modifiers);
}
