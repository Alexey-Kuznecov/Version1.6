
namespace UnityCommander.Abstractions.IO
{
    public class FileTransferItem
    {
        public required Guid Id;
        public required string SourcePath;
        public required string DestinationPath;
    }
}
