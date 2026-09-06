
using System.Collections.Concurrent;

namespace UnityCommander.Abstractions.IO
{
    public sealed class OperationRuntime
    {
        public OperationState State { get; }

        public ConcurrentDictionary<Guid, long> ItemCompletedBytes { get; }
            = new();

        public ConcurrentDictionary<Guid, byte> CompletedItems { get; }
            = new();

        public OperationRuntime(OperationState state)
        {
            State = state;
        }
    }
}
