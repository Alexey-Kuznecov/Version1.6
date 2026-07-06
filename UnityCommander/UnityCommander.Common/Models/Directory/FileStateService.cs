
using System;
using System.Collections.Concurrent;
using UnityCommander.Abstractions.IO;

namespace UnityCommander.Common.Models
{
    public class FileRuntimeService : IFileStateService
    {
        private ConcurrentDictionary<Guid, IFileState> _state 
            = new ConcurrentDictionary<Guid, IFileState>();

        private readonly IOperationIndex _index;

        public FileRuntimeService(IOperationIndex index)
        {
            _index = index;
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
    }
}
