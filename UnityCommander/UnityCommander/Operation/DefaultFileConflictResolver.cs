
using DryIoc;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Operation
{
    public sealed class DefaultFileConflictResolver : IFileConflictResolver
    {
        private readonly IWindowManager _windowManager;

        public DefaultFileConflictResolver(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public Task<FileConflictAction> ResolveAsync(
            FileConflict conflict,
            CancellationToken cancellationToken = default)
        {
            var result =
                _windowManager.ShowModalDialog<FileConflictResult>(
                    "core.file-conflict-dialog",
                    conflict);

            var action = result?.Action ?? FileConflictAction.Cancel;

            return Task.FromResult(action);
        }
    }
}
