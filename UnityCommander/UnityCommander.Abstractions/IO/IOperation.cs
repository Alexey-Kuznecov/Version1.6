
namespace UnityCommander.Abstractions.IO
{
    public interface IOperation
    {
        Guid Id { get; }

        List<FileTransferItem> Items { get; }

        OperationState? State { get; }
    }
}