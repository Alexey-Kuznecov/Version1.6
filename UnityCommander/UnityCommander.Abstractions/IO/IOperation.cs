
namespace UnityCommander.Abstractions.IO
{
    public interface IOperation
    {
        public Guid Id { get; }

        OperationState? State { get; }
    }
}