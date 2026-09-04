
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

        public Task CreateAsync(string creationId, string directory)
        {
            switch (creationId)
            {
                case "folder":
                    Directory.CreateDirectory(
                        Path.Combine(directory, "New Folder"));
                    break;

                case "text-file":
                    File.WriteAllText(
                        Path.Combine(directory, "New Text File.txt"),
                        string.Empty);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
