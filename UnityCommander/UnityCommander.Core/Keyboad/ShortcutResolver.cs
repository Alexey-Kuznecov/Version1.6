
using UnityCommander.Abstractions.Keyboard;

namespace UnityCommander.Core.Keyboad
{
    public class ShortcutResolver : IShortcutResolver
    {
        private readonly IShortcutRegistry _registry;
        private readonly IShortcutMapProvider _map;

        public ShortcutResolver(
            IShortcutRegistry shortcut,
            IShortcutMapProvider provider)
        {
            _registry = shortcut;
            _map = provider;
        }

        public bool TryResolve(
            ShortcutKey key,
            ShortcutModifiers mods,
            ShortcutScope scope,
            out string commandId)
        {
            commandId = null;

            if (!_map.TryGet(
                    new ShortcutGesture(key, mods),
                    out var shortcut))
            {
                return false;
            }

            if ((shortcut.Scopes & scope) == 0)
            {
                return false;
            }

            commandId = shortcut.CommandId;
            return true;
        }
    }
}
