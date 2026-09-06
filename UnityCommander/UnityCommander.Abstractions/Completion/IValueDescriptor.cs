
namespace UnityCommander.Abstractions.Completion
{
    public interface IValueDescriptor
    {
        string Name { get; }

        ArgumentValueType ValueType { get; }
    }
}
