
namespace UnityCommander.Abstractions.IO
{
    public interface IOperationIndex
    {
        void Register(IOperation op, IEnumerable<string> paths);

        void Unregister(Guid id);

        bool TryGetOperation(string path, out IOperation op);

        bool TryGetItem(string path, out FileTransferItem item);
    }
}