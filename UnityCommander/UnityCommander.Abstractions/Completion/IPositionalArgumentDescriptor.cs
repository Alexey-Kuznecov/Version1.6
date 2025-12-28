
namespace UnityCommander.Abstractions.Completion
{
    public interface IPositionalArgumentDescriptor
    {
        string Name { get; }
        ArgumentValueType ValueType { get; }
        bool IsRequired { get; }
        bool IsRepeatable { get; }
    }
}
