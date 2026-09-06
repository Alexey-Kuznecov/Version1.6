
using System.Windows;
using UnityCommander.Abstractions.Background;
using UnityCommander.Abstractions.IO;
using UnityCommander.Common.StatusBar;
using UnityCommander.Core.IO;
using UnityCommander.Modules.StatusBar.ViewModels;
using UnityCommander.WPF;

namespace UnityCommander.Modules.StatusBar.Services
{
    public class CopyMonitorService : IBackgroundService, IStatusBarProvider
    {
        private readonly IOperationProgressService _operationProgress;

        private readonly CopyProgressItem _item;

        public string Id => "core.copy.monitor.service";

        public string Name => "Copy Monitor Service";

        public bool IsRunning { get; private set; }

        public bool AutoStart => true;

        public string OwnerId => "core.background.service";

        public CopyMonitorService(
             IOperationProgressService operationProgress,
             ICopyOperationService operationService,
             IPopupService popup)
        {
            _operationProgress = operationProgress;

            _item = new CopyProgressItem();

            _item.Details =
                new CopyProgressViewModel(operationService);

            _item.Command = new DelegateCommand<FrameworkElement>(
                obj => popup.Show(obj, _item.Details));
        }

        public Task RunAsync(CancellationToken token)
        {
            _operationProgress.ProgressChanged += OnProgressChanged;
            _operationProgress.OperationCompleted += OperationCompleted;
            _operationProgress.AllOperationsCompleted += AllOperationsCompleted;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _operationProgress.ProgressChanged -= OnProgressChanged;

            return Task.CompletedTask;
        }

        private void OnProgressChanged(OperationState state)
        {
            var global = _operationProgress.GetGlobalState();

            if (global == null)
                return;

            _item.Progress = global.Percentage;
            _item.Speed = global.Speed;
        }

        private void OperationCompleted(OperationState state)
        {
        }

        private void AllOperationsCompleted()
        {
            _item.Progress = 0;
            _item.Speed = 0;
        }

        public IEnumerable<IStatusBarItem> GetItems()
        {
            yield return _item;
        }
    }
}
