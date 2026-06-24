
namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class ShortcutMapProvider : IShortcutMapProvider
    {
        private readonly IShortcutMapBuilder _builder;

        private volatile ShortcutMap _map;

        public ShortcutMapProvider(IShortcutMapBuilder builder)
        {
            _builder = builder;
            _map = _builder.Build();
        }

        public bool TryGet(
            ShortcutGesture gesture,
            out ShortcutDefinition? shortcut)
        {
            return _map.Map.TryGetValue(gesture, out shortcut);
        }

        public void Rebuild()
        {
            _map = _builder.Build();
        }
    }
}
