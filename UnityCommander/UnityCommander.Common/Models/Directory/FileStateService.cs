
using System;
using System.Collections.Concurrent;
using System.IO;
using UnityCommander.Abstractions;
using UnityCommander.Abstractions.IO;
using UnityCommander.Common.Events;
using UnityCommander.Core.Events;

namespace UnityCommander.Common.Models
{
    public class FileRuntimeService : IFileStateService
    {
        private ConcurrentDictionary<Guid, IFileState> _state 
            = new ConcurrentDictionary<Guid, IFileState>();

        private readonly IOperationIndex _index;

        private readonly IEventBus _eventBus;

        public FileRuntimeService(
            IOperationIndex index,
            IEventBus eventBus)
        {
            _eventBus = eventBus;
            _index = index;

            _eventBus.Subscribe<CopyProgressEvent>(OnProgressReport);
            _eventBus.Subscribe<CopyCompleteEvent>(OnCompleteFile);
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
            var fileName = Path.GetFileName(e.Info.Source);
            var destinationFilePath = Path.Combine(e.Info.Destination, fileName);

            this.Set(e.Info.ItemId, new FileState()
            {
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
            this.Remove(e.Info.ItemId);
        }
    }
}
