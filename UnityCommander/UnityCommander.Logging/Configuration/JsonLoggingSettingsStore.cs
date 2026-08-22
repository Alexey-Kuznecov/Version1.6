
using System.Text.Json;

namespace UnityCommander.Logging.Configuration
{
    public sealed class JsonLoggingSettingsStore : ILoggingSettingsStore
    {
        private readonly string _path;

        public JsonLoggingSettingsStore(string? path = null)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public LoggingSettings Load()
        {
            if (!File.Exists(_path))
                return new LoggingSettings();

            var json = File.ReadAllText(_path);

            var settings =
                JsonSerializer.Deserialize<LoggingSettings>(json)
                ?? new LoggingSettings();

            settings.DisabledCategories = new HashSet<string>(
                settings.DisabledCategories,
                StringComparer.OrdinalIgnoreCase);

            settings.DisabledScopes = new HashSet<string>(
                settings.DisabledScopes,
                StringComparer.OrdinalIgnoreCase);

            return settings;
        }

        public void Save(LoggingSettings settings)
        {
            var directory = Path.GetDirectoryName(_path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_path, json);
        }
    }
}
