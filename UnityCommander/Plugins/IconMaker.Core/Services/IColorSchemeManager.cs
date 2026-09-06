
using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IColorSchemeManager
    {
        IReadOnlyCollection<ColorScheme> GetAll();

        ColorScheme? Get(string name);

        void Add(ColorScheme scheme);

        bool Remove(string name);

        bool Rename(string oldName, string newName);
    }
}
