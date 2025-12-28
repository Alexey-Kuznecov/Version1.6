
namespace UnityCommander.Abstractions.Completion
{
    public interface ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<ICommandVariant> Variants { get; }
    }
}
