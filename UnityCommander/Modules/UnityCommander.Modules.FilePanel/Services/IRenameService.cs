
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Services
{
    public interface IRenameService
    {
        Task RenameAsync(
            string sourcePath,
            string newName);
    }
}
