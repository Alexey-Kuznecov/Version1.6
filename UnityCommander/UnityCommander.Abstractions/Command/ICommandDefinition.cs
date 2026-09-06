
namespace UnityCommander.Abstractions.Command
{
    public interface ICommandDefinition : IOwned
    {
        string? Id { get; }
        Type CommandType { get; }
    }
}
