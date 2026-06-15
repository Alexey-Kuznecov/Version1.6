
using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IIconThemeService
    {
        IconTheme CurrentTheme { get; }

        void SetCurrentTheme(string id);


        event Action<string> ThemeChanged;

        IconTheme GetTheme(string id);

        IReadOnlyCollection<IconTheme> GetThemes();

        void CreateTheme(string name);

        void DeleteTheme(string id);

        void RenameTheme(string id, string name);

        void SetPack(string id, string packId);

        void SetScale(string id, double scale);

        void SetColorScheme(string id, string schemeId);

        void SetMonochrome(string id, bool enabled);

        void Save(string id);

        void SaveAll();
    }
}
