
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutMap
    {
        public required Dictionary<ShortcutGesture, ShortcutDefinition> Map { get; init; }
    }
}
