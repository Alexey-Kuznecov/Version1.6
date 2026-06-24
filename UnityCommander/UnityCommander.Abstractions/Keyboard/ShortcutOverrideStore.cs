
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutOverrideStore : IShortcutOverrideStore
    {
        private readonly Dictionary<string, ShortcutOverride> _map;

        public ShortcutOverrideStore(JsonShortcutOverrideStorage overrideStorage)
        {
            _map = overrideStorage.Load();
        }

        public IReadOnlyCollection<ShortcutOverride> GetAll()
            => _map.Values;

        public bool TryGet(string commandId, out ShortcutOverride value)
            => _map.TryGetValue(commandId, out value);

        public void Set(ShortcutOverride value)
            => _map[value.CommandId] = value;

        public bool TrySet(ShortcutOverride value)
        {
            return _map.TryAdd(value.CommandId, value);
        }

        public void Remove(string commandId)
            => _map.Remove(commandId);

        public Dictionary<string, ShortcutOverride> GetSnapshot()
            => new(_map);
    }
}
