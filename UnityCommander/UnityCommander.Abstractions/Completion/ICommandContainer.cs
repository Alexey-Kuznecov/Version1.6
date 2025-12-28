
namespace UnityCommander.Abstractions.Completion
{
    public interface ICommandContainer
    {
        string Name { get; }
        IReadOnlyList<ICommandDescriptor> Commands { get; }
    }
}
