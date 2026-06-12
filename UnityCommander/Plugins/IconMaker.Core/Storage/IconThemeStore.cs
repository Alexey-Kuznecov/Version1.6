
using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public sealed class IconThemeStore : IIconThemeStore
    {
        private readonly IIconThemeStorage _storage;

        private readonly Dictionary<string, IconTheme> _cache = new();

        private readonly HashSet<string> _dirtyThemes = new();

        public IconThemeStore(IIconThemeStorage storage)
        {
            _storage = storage;
        }

        public IconTheme Get(string id)
        {
            if (_cache.TryGetValue(id, out var theme))
                return theme;

            theme = _storage.Load(id);

            _cache[id] = theme;

            return theme;
        }

        public IReadOnlyCollection<IconTheme> GetLoadedThemes()
        {
            return _cache.Values;
        }

        public void Add(IconTheme theme)
        {
            _cache[theme.Id] = theme;

            _dirtyThemes.Add(theme.Id);
        }

        public void Remove(string id)
        {
            _cache.Remove(id);

            _dirtyThemes.Remove(id);

            _storage.Delete(id);
        }

        public void Save(string id)
        {
            if (!_cache.TryGetValue(id, out var theme))
                return;

            _storage.Save(theme);

            _dirtyThemes.Remove(id);
        }

        public void SaveAll()
        {
            foreach (var id in _dirtyThemes.ToList())
            {
                Save(id);
            }
        }
    }
}
