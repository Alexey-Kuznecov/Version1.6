
using System;
using System.Collections.Concurrent;
using System.IO;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.IO;
using UnityCommander.Common.Events;
using UnityCommander.Common.Models;

namespace UnityCommander.Services
{
    public class FileRuntimeService : IFileStateService
    {
        private ConcurrentDictionary<Guid, IFileState> _state 
            = new ConcurrentDictionary<Guid, IFileState>();

        private readonly IOperationIndex _index;

        private readonly IEventBus _eventBus;

        private readonly IFileActivityService _fileActivityService;

        public FileRuntimeService(
            IFileActivityService fileActivity,
            IOperationIndex index,
            IEventBus eventBus)
        {
            _fileActivityService = fileActivity;
            _eventBus = eventBus;
            _index = index;
            _eventBus.Subscribe<CopyProgressEvent>(OnProgressReport);
            _eventBus.Subscribe<CopyCompleteEvent>(OnCompleteFile);
            _eventBus.Subscribe<FileStatusChangedEvent>(OnFileStatus);
        }

        public IFileState GetState(string path)
        {
            if (_index.TryGetItem(path, out var item))
            {
                if (_state.TryGetValue(item.Id, out var state))

                    return state;
            }

            return null;
        }

        public void Remove(Guid operationId)
        {
            _state.TryRemove(operationId, out _);
        }

        public void Set(Guid operationId, IFileState state)
        {
            _state[operationId] = state;
        }

        private void OnProgressReport(object sender, CopyProgressEvent e)
        {
            _fileActivityService.Activate(
                e.Info.ItemId,
                e.Info.Source,
                e.Info.Target);

            var destinationFilePath = e.Info.Target;

            this.Set(e.Info.ItemId, new FileState()
            {
                Status = MapOperationStatus(e.Info.Status),
                SourcePath = e.Info.Source,
                DestinationPath = destinationFilePath,
                IsCopying = true,
                RemainingTime = e.Info.TotalTimeLeft,
                Progress = (int)Math.Round(e.Info.TotalPercentage),
                Speed = (long)e.Info.AverageSpeed
            });
        }

        private void OnCompleteFile(object sender, CopyCompleteEvent e)
        {
            e.Info.Status = FileTransferStatus.Completed;

            _fileActivityService.Activate(
                e.Info.ItemId,
                e.Info.Source,
                e.Info.Target);

            //this.Remove(e.Info.ItemId);
            //_index.Unregister(e.Info.ItemId);
        }

        private void OnFileStatus(
            object? sender,
            FileStatusChangedEvent e)
        {
            if (e.Status == FileTransferStatus.Skipped || e.Status == FileTransferStatus.Failed)
                _fileActivityService.Deactivate(e.ItemId);

            var state = (FileState)GetState(e.SourcePath);

            if (state is null)
            {
                Set(e.ItemId, new FileState
                {
                    SourcePath = e.SourcePath,
                    DestinationPath = e.DestinationPath,
                    Status = MapOperationStatus(e.Status)
                });

                return;
            }

            state.Status = MapOperationStatus(e.Status);
        }

        private static OperationStatus MapOperationStatus(FileTransferStatus status)
        {
            return status switch
            {
                FileTransferStatus.Copying => OperationStatus.InProgress,
                FileTransferStatus.Skipped => OperationStatus.Skipped,
                FileTransferStatus.Pending => OperationStatus.Pending,
                FileTransferStatus.Cancelled => OperationStatus.Canceled,
                FileTransferStatus.Failed => OperationStatus.Failed,
                _ => OperationStatus.Completed
            };
        }
    }
}
