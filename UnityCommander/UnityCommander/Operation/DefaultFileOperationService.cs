
using System.Threading.Tasks;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Settings;
using UnityCommander.Settings.Abstactions;

namespace UnityCommander.Operation
{
    public sealed class DefaultFileOperationService
       : IFileOperationService
    {
        private readonly IWindowManager _windowManager;

        private readonly IFileCopyEngine _engine;

        private readonly ISettingsService _settings;

        public DefaultFileOperationService(
            IFileCopyEngine engine, 
            IWindowManager windowManager, 
            ISettingsService settings)
        {
            _settings = settings;
            _engine = engine;
            _windowManager = windowManager;
        }

        public async Task CopyAsync(FileOperationRequest request)
        {
            var result = _windowManager.ShowModalDialog<CopyDialogResult>(
                "core.copy-dialog",
                request);

            if (result is null || !result.Accepted)
                return;

            var task = _engine.StartAsync(result.Request);

            if (_settings.Get(GeneralSettings.ShowCopyProgressDialog))
            {
                _windowManager.ShowModalDialog<CopyDialogResult>(
                    "core.copy-progress-dialog",
                    result.Request);
            }

            await task;
        }
    }
}
