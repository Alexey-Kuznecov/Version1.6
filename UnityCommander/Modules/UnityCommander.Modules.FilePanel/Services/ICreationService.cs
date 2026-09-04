
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityCommander.Modules.FilePanel.Models;

namespace UnityCommander.Modules.FilePanel.Services
{
    public interface ICreationService
    {
        IReadOnlyList<CreationDefinition> GetAvailable();

        Task CreateAsync(
            string creationId,
            string directory);
    }
}
