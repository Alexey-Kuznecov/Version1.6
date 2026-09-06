
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Services
{
    public sealed class RenameManager : IRenameManager
    {
        private readonly IRenameService _renameService;

        private string? _sourcePath;

        public bool IsActive =>
            _sourcePath is not null;

        public RenameManager(IRenameService renameService)
        {
            _renameService = renameService;
        }

        public void Start(string sourcePath)
        {
            if (IsActive)
                Cancel();

            _sourcePath = sourcePath;
        }

        public async Task CommitAsync(string newName)
        {
            if (_sourcePath is null)
                return;

            await _renameService.RenameAsync(
                _sourcePath,
                newName);

            _sourcePath = null;
        }

        public void Cancel()
        {
            _sourcePath = null;
        }
    }
}
