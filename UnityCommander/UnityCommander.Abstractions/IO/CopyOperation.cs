
namespace UnityCommander.Abstractions.IO
{
    public class CopyOperation : IOperation
    {
        public Guid Id { get; set; }

        public List<FileTransferItem> Items 
            { get; set; } = new List<FileTransferItem>();

        public OperationState? State { get; }
    }
}
