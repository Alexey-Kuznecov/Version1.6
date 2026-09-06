
using IconMaker.Core.Models;

namespace IconMaker.Core.Services
{
    public interface IIconPackManager
    {
        IReadOnlyCollection<IconPack> GetAll();

        IconPack? Get(string name);

        void Add(IconPack pack);

        bool Remove(string name);

        bool Rename(string oldName, string newName);

        bool Exists(string name);
    }
}
