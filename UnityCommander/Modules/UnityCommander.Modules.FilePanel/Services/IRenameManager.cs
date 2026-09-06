
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Services
{
    public interface IRenameManager
    {
        bool IsActive { get; }

        void Start(string sourcePath);

        Task CommitAsync(string newName);

        void Cancel();
    }
}
