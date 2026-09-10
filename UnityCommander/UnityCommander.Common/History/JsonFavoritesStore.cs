
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityCommander.Abstractions.History;

namespace UnityCommander.Common.History
{
    public sealed class JsonUserFavoritesStore : IUserFavoriteStore
    {
        private readonly string _filePath;

        public JsonUserFavoritesStore(UnityCommanderPath commanderPath)
        {
            _filePath = Path.Combine(
                commanderPath.DataDirectory,
                "favorites.json");
        }

        public IReadOnlyList<string> Load()
        {
            if (!File.Exists(_filePath))
                return [];

            try
            {
                var json = File.ReadAllText(_filePath);

                return JsonSerializer.Deserialize<List<string>>(json)
                       ?? [];
            }
            catch
            {
                return [];
            }
        }

        public void Save(IReadOnlyCollection<string> paths)
        {
            var json = JsonSerializer.Serialize(
                paths,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText(_filePath, json);
        }
    }
}
