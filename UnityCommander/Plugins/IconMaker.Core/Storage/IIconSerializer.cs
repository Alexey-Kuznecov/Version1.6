
using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public interface IIconSerializer
    {
        string Serialize(IconPack pack);

        IconPack Deserialize(string data);
    }
}
