
using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public interface IIconThemeStorage
    {
        IconTheme Load(string id);

        void Save(IconTheme theme);

        void Delete(string id);
    }
}
