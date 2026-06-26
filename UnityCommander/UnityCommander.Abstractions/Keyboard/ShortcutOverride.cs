
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutOverride
    {
        public string? CommandId { get; init; }

        public required ShortcutKey Key { get; init; }

        public required ShortcutModifiers Modifiers { get; init; }
    }
}
