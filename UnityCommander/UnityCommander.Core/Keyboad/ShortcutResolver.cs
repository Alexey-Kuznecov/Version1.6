
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Core.Keyboad
{
    public class ShortcutResolver : IShortcutResolver
    {
        private readonly IShortcutRegistry _registry;

        public ShortcutResolver(IShortcutRegistry shortcut)
        {
            _registry = shortcut;
        }

        public bool TryResolve(
            ShortcutKey key,
            ShortcutModifiers mods,
            ShortcutScope scope,
            out string commandId)
        {
            foreach (var kv in _registry.GetAll())
            {
                if (kv.Key == key &&
                    kv.Modifiers == mods &&
                    kv.Scope == scope)
                {
                    commandId = kv.CommandId;
                    return true;
                }
            }

            foreach (var kv in _registry.GetAll())
            {
                if (kv.Key == key &&
                    kv.Modifiers == mods &&
                    kv.Scope == ShortcutScope.Global)
                {
                    commandId = kv.CommandId;
                    return true;
                }
            }

            commandId = null;
            return false;
        }
    }
}
