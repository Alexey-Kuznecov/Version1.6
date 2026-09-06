
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutDefinition
    {
        public required string CommandId { get; init; }
        
        public string? Description { get; init; }

        public ShortcutKey Key { get; init; }

        public ShortcutModifiers Modifiers { get; init; }

        public ShortcutScope Scopes { get; init; }
    }
}
