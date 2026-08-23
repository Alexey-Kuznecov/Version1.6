
using System.IO;
using UnityCommander.Abstractions.IO;
using UnityCommander.Core.IO.Operations;

namespace UnityCommander.Modules.StatusBar.ViewModels
{
    public class CopyOperationViewModel : BindableBase
    {
        private readonly CopyManager _manager;
        private readonly IOperationProgressService _progressService;

        public Guid Id => _manager.Id;

        private string _title = string.Empty;

        private long _speed;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string DisplayName =>
           $"{FileProgress:0}% • {Speed / 1024 / 1024} MB/s";

        public int OperationProgressPercent =>
            (int)Math.Round(OperationProgress);

        private double _operationProgress;

        public double OperationProgress
        {
            get => _operationProgress;
            set
            {
                if (SetProperty(ref _operationProgress, value))
                {
                    RaisePropertyChanged(nameof(DisplayName));
                    RaisePropertyChanged(nameof(OperationProgressPercent));
                }
            }
        }

        private double _fileProgress;

        public double FileProgress
        {
            get => _fileProgress;
            set => SetProperty(ref _fileProgress, value);
        }

        public long Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }

        public DelegateCommand PauseCommand 
            => new DelegateCommand(() => _manager.Pause());

        public DelegateCommand ResumeCommand 
            => new DelegateCommand(() => _manager.Resume());

        public DelegateCommand CancelCommand 
            => new DelegateCommand(() => _manager.Cancel());

        public CopyOperationViewModel(CopyManager manager, IOperationProgressService progressService)
        {
            _manager = manager;
            _progressService = progressService;

            _manager.CopyFileReport += CopyFileReport;
            _manager.FileCompleted += FileCompleted;

            _progressService.ProgressChanged += ProgressChanged;
            _progressService.OperationCompleted += OperationCompleted;
        }

        private void ProgressChanged(OperationState state)
        {
            if (state.OperationId != Id)
                return;

            OperationProgress = state.Percentage;
            Speed = state.Speed;
        }

        private void OperationCompleted(OperationState state)
        {
            //var operation = _progressService.Get(state.OperationId);

            //if (operation == null)
            //    return;

            //OperationProgress = operation.Percentage;
            //Speed = operation.Speed;
        }

        private void FileCompleted(CopyInfo info)
        {
            Title = Path.GetFileName(info.Source);
        }

        private void CopyFileReport(CopyInfo info)
        {
            Title = Path.GetFileName(info.Source);
            FileProgress = info.Percentage;
            Speed = (long)info.AverageSpeed;
        }
    }
}
