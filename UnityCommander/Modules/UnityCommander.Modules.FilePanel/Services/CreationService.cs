
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.Services
{
    public class CreationService : ICreationService
    {
        private readonly Dictionary<string, CreationDefinition> _definitions = new()
        {
            ["folder"] = new("folder", "Folder", "Folder"),
            ["text-file"] = new("text-file", "Text file", "FileText")
        };

        public IReadOnlyList<CreationDefinition> GetAvailable()
            => _definitions.Values.ToList();

        public Task CreateAsync(CreationContext creation)
        {
            var name = creation.Type == CreationType.File
                ? Path.ChangeExtension(
                    creation.InputName,
                    creation.Extension)
                : creation.InputName;

            var path = Path.Combine(
                creation.TargetDirectory,
                name);

            if (File.Exists(path) || Directory.Exists(path))
                throw new IOException($"Object already exists: {path}");

            switch (creation.Type)
            {
                case CreationType.Directory:
                    Directory.CreateDirectory(path);
                    break;

                case CreationType.File:
                    File.WriteAllText(path, string.Empty);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
