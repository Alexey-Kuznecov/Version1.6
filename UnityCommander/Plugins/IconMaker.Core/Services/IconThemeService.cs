
using IconMaker.Core.Models;
using IconMaker.Core.Storage;

namespace IconMaker.Core.Services
{
    public sealed class IconThemeService : IIconThemeService
    {
        private readonly IIconThemeStore _store;

        public IconThemeService(IIconThemeStore store)
        {
            _store = store;
        }

        public IconTheme GetTheme(string id)
        {
            return _store.Get(id);
        }

        public IReadOnlyCollection<IconTheme> GetThemes()
        {
            return _store.GetLoadedThemes();
        }

        public void CreateTheme(string name)
        {
            var theme = new IconTheme
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Scale = 1.0,
                IsMonochrome = false
            };

            _store.Add(theme);
        }

        public void DeleteTheme(string id)
        {
            _store.Remove(id);
        }

        public void RenameTheme(string id, string name)
        {
            var theme = _store.Get(id);

            theme.Name = name;
        }

        public void SetPack(string id, string packId)
        {
            var theme = _store.Get(id);

            theme.PackId = packId;
        }

        public void SetScale(string id, double scale)
        {
            var theme = _store.Get(id);

            theme.Scale = scale;
        }

        public void SetColorScheme(string id, string schemeId)
        {
            var theme = _store.Get(id);

            theme.ColorSchemeId = schemeId;
        }

        public void SetMonochrome(string id, bool enabled)
        {
            var theme = _store.Get(id);

            theme.IsMonochrome = enabled;
        }

        public void Save(string id)
        {
            _store.Save(id);
        }

        public void SaveAll()
        {
            _store.SaveAll();
        }
    }
}
