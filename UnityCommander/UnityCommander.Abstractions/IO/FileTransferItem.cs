
namespace UnityCommander.Abstractions.IO
{
    public class FileTransferItem
    {
        public required Guid Id;

        public required FileTransferStatus Status;
        public required string SourcePath;
        public required string DestinationPath;

        public bool ShouldCleanupDestination { get; set; }
        public long Length { get; set; }
        public FileTransferItemType ItemType { get; set; }
    }
}
