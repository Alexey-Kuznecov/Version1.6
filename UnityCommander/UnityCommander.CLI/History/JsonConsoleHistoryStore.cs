
using System.Text.Json;
using UnityCommander.CLI.History;

public sealed class JsonConsoleHistoryStore : IConsoleHistoryStore
{
    private readonly string _path;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public JsonConsoleHistoryStore(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public IReadOnlyList<string> Load()
    {
        if (!File.Exists(_path))
            return Array.Empty<string>();

        var json = File.ReadAllText(_path);

        if (string.IsNullOrWhiteSpace(json))
            return Array.Empty<string>();

        var commands = JsonSerializer.Deserialize<List<string>>(json);

        return commands ?? [];
    }

    public void Save(IReadOnlyList<string> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        var directory = Path.GetDirectoryName(_path);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(commands, JsonOptions);

        File.WriteAllText(_path, json);
    }
}