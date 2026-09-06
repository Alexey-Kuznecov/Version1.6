
using System.Text.Json;

namespace UnityCommander.Abstractions.Keyboard
{
    public sealed class JsonShortcutOverrideStorage
    {
        private readonly string _path;

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public JsonShortcutOverrideStorage(string path)
        {
            _path = path;
        }

        public Dictionary<string, ShortcutOverride> Load()
        {
            if (!File.Exists(_path))
                return new Dictionary<string, ShortcutOverride>();

            var json = File.ReadAllText(_path);

            return JsonSerializer.Deserialize<Dictionary<string, ShortcutOverride>>(json)
                   ?? new Dictionary<string, ShortcutOverride>();
        }

        public void Save(Dictionary<string, ShortcutOverride> shotcuts)
        {
            var json = JsonSerializer.Serialize(shotcuts, Options);

            File.WriteAllText(_path, json);
        }
    }
}
