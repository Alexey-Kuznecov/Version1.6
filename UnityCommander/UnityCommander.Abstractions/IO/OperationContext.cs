
namespace UnityCommander.Abstractions.IO
{
    public class OperationContext
    {
        public Guid OperationId { get; init; }

        public CancellationTokenSource? Cancellation { get; init; }

        public CopyOperation? Operation { get; init; }

        public CopyInfo? Info { get; init; }
    }
}
