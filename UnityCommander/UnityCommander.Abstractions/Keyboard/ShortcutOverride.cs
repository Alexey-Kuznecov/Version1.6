
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutOverride
    {
        public string? CommandId { get; init; }

        public ShortcutKey Key { get; init; }

        public ShortcutModifiers Modifiers { get; init; }
    }
}
