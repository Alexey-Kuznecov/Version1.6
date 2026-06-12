
using IconMaker.Core.Models;
using IconMaker.Core.Storage;
using System.Text.Json;

namespace IconMaker.Core.ImportExport
{
    public class JsonIconThemeSerializer : IThemeSerializer
    {
        public string Serialize(IconTheme pack)
            => JsonSerializer.Serialize(pack);

        public IconTheme Deserialize(string data)
            => JsonSerializer.Deserialize<IconTheme>(data)!;
    }
}
