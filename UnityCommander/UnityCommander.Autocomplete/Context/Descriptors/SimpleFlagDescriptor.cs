using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class SimpleFlagDescriptor : IFlagDescriptor
    {
        public string Name { get; init; } = string.Empty;
        public string? ShortName { get; init; }
        public ArgumentValueType? ValueType { get; init; }
        public bool IsRepeatable { get; init; }
        public bool RequiresValue { get; }

        public SimpleFlagDescriptor(
            string name,
            string? shortName,
            bool requiresValue,
            ArgumentValueType? valueType = null)
        {
            Name = name;
            ShortName = shortName;
            RequiresValue = requiresValue;
            ValueType = valueType;
        }
    }
}
    