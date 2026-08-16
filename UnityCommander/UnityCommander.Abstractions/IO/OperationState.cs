
namespace UnityCommander.Abstractions.IO
{
    public class OperationState
    {
        public Guid OperationId { get; init; }

        public long TotalBytes { get; set; }

        public long CompletedBytes { get; set; }

        public int TotalFiles { get; set; }

        public long Speed { get; set; }

        public OperationStatus Status { get; set; }

        public double Percentage =>
            TotalBytes > 0
                ? (double)CompletedBytes / TotalBytes * 100
                : 0;

        public int TotalItems { get; set; }
        
        public int CompletedItems { get; set; }
    }
}
