
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Core.Keyboad
{
    public interface IShortcutResolver
    {
        bool TryResolve(
            ShortcutKey key,
            ShortcutModifiers mods,
            ShortcutScope scope,
            out string commandId);
    }
}
