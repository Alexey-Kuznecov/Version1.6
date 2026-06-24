
using System.IO;
using System.Text.Json;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Settings.Core
{
    public sealed class JsonSettingsStore : ISettingsStore
    {
        private readonly string _path;

        public JsonSettingsStore(string path)
        {
            _path = path;
        }

        public Dictionary<string, object?> Load()
        {
            if (!File.Exists(_path))
                return new();

            var json = File.ReadAllText(_path);

            return JsonSerializer.Deserialize<Dictionary<string, object?>>(json)
                   ?? new();
        }

        public void Save(Dictionary<string, object?> values)
        {
            var json = JsonSerializer.Serialize(values, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_path, json);
        }
    }
}
