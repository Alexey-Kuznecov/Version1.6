
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class SimpleFlagDescriptor : IFlagDescriptor
    {
        public string Name { get; init; } = string.Empty;
        public string? ShortName { get; init; }
        public ArgumentValueType ValueType { get; init; }
        public ValueSeparator Separator { get; }
        public bool IsRepeatable { get; init; }
        public bool RequiresValue { get; }

        public SimpleFlagDescriptor(
         string name,
         string? shortName,
         bool requiresValue,
         bool isRepeatable = false,
         ArgumentValueType valueType = ArgumentValueType.String,
         ValueSeparator valueSeparator = ValueSeparator.Space)
        {
            Name = name;
            ShortName = shortName;
            RequiresValue = requiresValue;
            IsRepeatable = isRepeatable;
            ValueType = valueType;
            Separator = valueSeparator;
        }
    }
}
    