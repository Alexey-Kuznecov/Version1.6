using IconMaker.Core.Models;
using System.Text.Json;

namespace IconMaker.Core.Storage
{
    public class JsonIconSerializer : IIconSerializer
    {
        public string Serialize(IconPack pack)
            => JsonSerializer.Serialize(pack);

        public IconPack Deserialize(string data)
            => JsonSerializer.Deserialize<IconPack>(data)!;
    }
}
