
using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public interface IIconThemeStore
    {
        IconTheme Get(string id);

        IReadOnlyCollection<IconTheme> GetLoadedThemes();

        void Add(IconTheme theme);

        void Remove(string id);

        void Save(string id);
        
        void SaveAll();
    }
}
