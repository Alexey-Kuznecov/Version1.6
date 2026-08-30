
namespace UnityCommander.Abstractions.Completion
{
    public interface IPositionalArgumentDescriptor : IValueDescriptor
    {
        bool IsRequired { get; }
        bool IsRepeatable { get; }
    }
}
