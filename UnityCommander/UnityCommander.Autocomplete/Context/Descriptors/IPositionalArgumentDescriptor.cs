using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public interface IPositionalArgumentDescriptor
    {
        string Name { get; }
        ArgumentValueType ValueType { get; }
        bool IsRequired { get; }
        bool IsRepeatable { get; } // например: files...
    }
}
