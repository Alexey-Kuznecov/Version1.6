using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class ArgumentDescriptor : IPositionalArgumentDescriptor
    {
        public string Name { get; init; } = string.Empty;
        public ArgumentValueType ValueType { get; init; }
        public bool IsRequired { get; init; }
        public bool IsRepeatable { get; init; }
    }
}
