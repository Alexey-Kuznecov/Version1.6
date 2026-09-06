
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Core.Keyboad
{
    public sealed class ScopeEntry
    {
        public required object Owner { get; init; }

        public required ShortcutScope Scope { get; init; }
    }
}
