
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutMapBuilder : IShortcutMapBuilder
    {
        private readonly IShortcutRegistry _registry;
        private readonly IShortcutOverrideStore _overrides;

        public ShortcutMapBuilder(
            IShortcutRegistry registry,
            IShortcutOverrideStore overrides)
        {
            _registry = registry;
            _overrides = overrides;
        }

        public ShortcutMap Build()
        {
            var map = new Dictionary<ShortcutGesture, ShortcutDefinition>();

            var overrideDict =
                _overrides.GetAll()
                    .ToDictionary(x => x.CommandId);

            foreach (var def in _registry.GetAll())
            {
                if (overrideDict.TryGetValue(def.CommandId, out var ovr))
                {
                    map[new ShortcutGesture(
                        ovr.Key,
                        ovr.Modifiers)] = def;
                }
                else
                {
                    map[new ShortcutGesture(
                        def.Key,
                        def.Modifiers)] = def;
                }
            }

            return new ShortcutMap { Map = map };
        }
    }
}
