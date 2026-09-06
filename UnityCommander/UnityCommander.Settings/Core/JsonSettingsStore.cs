
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

        public Dictionary<string, object?> Load(
            Dictionary<string, SettingDefinition> definitions)
        {
            if (!File.Exists(_path))
                return new();

            var json = File.ReadAllText(_path);

            var values =
                JsonSerializer.Deserialize<Dictionary<string, object?>>(json)
                ?? new();

            foreach (var definition in definitions.Values)
            {
                if (values.TryGetValue(definition.Key, out var value) &&
                    value is JsonElement element)
                {
                    values[definition.Key] =
                        JsonSerializer.Deserialize(
                            element.GetRawText(),
                            definition.ValueType);
                }
            }

            return values;
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
