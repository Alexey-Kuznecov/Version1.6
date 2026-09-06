
using System.Collections.Generic;
using System.IO;

namespace UnityCommander.Modules.BottomPanel.Console
{
    using System.Text.Json;

    public sealed class ConsoleProfileStore : IConsoleProfileStore
    {
        private const string FileName = "console-profiles.json";
        private readonly string _path;

        private readonly List<ConsoleProfile> _profiles;

        public ConsoleProfileStore()
        {
            _path = Path.Combine(Directory.GetCurrentDirectory(), "config", FileName);
            _profiles = LoadProfiles() ?? new();
        }

        public IReadOnlyList<ConsoleProfile> Load()
        {
            return _profiles;
        }

        public void Save(ConsoleProfile profile)
        {
            var index = _profiles.FindIndex(x => x.ConsoleId == profile.ConsoleId);

            if (index >= 0)
                _profiles[index] = profile;
            else
                _profiles.Add(profile);

            SaveProfiles();
        }

        private void SaveProfiles()
        {
            var json = JsonSerializer.Serialize(
                _profiles,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_path, json);
        }

        private List<ConsoleProfile> LoadProfiles()
        {
            if (!File.Exists(_path))
                return new();

            try
            {
                var json = File.ReadAllText(_path);

                return JsonSerializer.Deserialize<List<ConsoleProfile>>(json)
                       ?? new();
            }
            catch
            {
                // Повреждённый файл профилей не должен ломать запуск приложения.
                return new();
            }
        }
     }
}
