using IconMaker.Core.Models;

namespace IconMaker.Core.Storage
{
    public interface IThemeSerializer
    {
        string Serialize(IconTheme pack);

        IconTheme Deserialize(string data);
    }
}