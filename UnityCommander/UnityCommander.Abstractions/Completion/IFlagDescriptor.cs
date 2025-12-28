
namespace UnityCommander.Abstractions.Completion
{
    public interface IFlagDescriptor
    {
        string Name { get; }          // --force
        string? ShortName { get; }    // -f
        ArgumentValueType? ValueType { get; }
        bool IsRepeatable { get; }    // --tag a --tag b
        bool RequiresValue { get; }
    }
}