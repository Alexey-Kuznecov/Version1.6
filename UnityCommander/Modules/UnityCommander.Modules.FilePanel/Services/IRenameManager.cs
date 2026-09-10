
using System.Threading.Tasks;
using UnityCommander.Common.Models.Directory;

namespace UnityCommander.Modules.FilePanel.Services
{
    public interface IRenameManager
    {
        bool IsActive { get; }

        void Start();

        void Start(BaseDirectory item);

        Task CommitAsync(string newName);

        void Cancel();
    }
}
