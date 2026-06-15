
using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public interface IIconStorage
    {
        IEnumerable<string> GetPackIds();

        IEnumerable<(string Id, string Name)> GetPackHeaders();

        IconPack Load(string name);

        void Save(IconPack pack);

        void Delete(string name);
    }
}
