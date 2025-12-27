using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
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