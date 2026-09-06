
using AdvancedCopyFiles.Views;
using UnityCommander.Abstractions.Dialog;
using UnityCommander.Abstractions.Overrides;
using UnityCommander.Copying;
using UnityCommander.Copying.Sessions;

namespace AdvancedCopyFiles.Core
{
    public sealed class AdvancedFileCopyEngine : IFileOperationService
    {
        private readonly OpenManager _openManager;
        private readonly CopySessionManager _sessionManager;
        private readonly ICopySettingsBuilder _builder;
        private readonly IWindowManager _windowManager;

        public AdvancedFileCopyEngine(
            OpenManager openManager, 
            CopySessionManager sessionManager,
            ICopySettingsBuilder builder, 
            IWindowManager windowManager)
        {
            _openManager = openManager;
            _sessionManager = sessionManager;
            _builder = builder;
            _windowManager = windowManager;
        }

        public async Task CopyAsync(FileOperationRequest request)
        {
            _windowManager.ShowDialog<MainView>(request);

            await Task.CompletedTask;
        }
    }
}
