
using System.Threading.Tasks;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;

namespace UnityCommander.Operation
{
    public sealed class DefaultFileOperationService
       : IFileOperationService
    {
        private readonly IWindowManager _windowManager;

        private readonly IFileCopyEngine _engine;

        public DefaultFileOperationService(IFileCopyEngine engine, IWindowManager windowManager)
        {
            _engine = engine;
            _windowManager = windowManager;
        }

        public Task CopyAsync(FileOperationRequest request)
        {
            var result =
                _windowManager.ShowModalDialog<CopyDialogResult>(
                       "core.copy-dialog",
                       request);

            if (result is null || !result.Accepted)
                return Task.CompletedTask;

            _engine.StartAsync(result.Request);

            _windowManager.ShowModalDialog<CopyDialogResult>(
                   "core.copy-progress-dialog",
                   request);

            return Task.CompletedTask;
        }
    }
}
