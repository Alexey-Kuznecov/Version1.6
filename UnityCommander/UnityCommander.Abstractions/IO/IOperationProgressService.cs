
namespace UnityCommander.Abstractions.IO
{
    public interface IOperationProgressService
    {
        event Action<OperationState>? ProgressChanged;
        event Action<OperationState>? OperationCompleted;
        event Action? AllOperationsCompleted;

        void Register(CopyOperation operation);
        
        void Unregister(Guid operationId);

        OperationState? Get(Guid operationId);

        IReadOnlyCollection<OperationState> GetAll();

        OperationState? GetGlobalState();
    }
}
