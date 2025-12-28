
using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class SimplePositionalArgumentDescriptor : IPositionalArgumentDescriptor
    {
        public string Name { get; }
        public ArgumentValueType ValueType { get; }
        public bool IsRequired { get; }
        public bool IsRepeatable { get; }
        public SimplePositionalArgumentDescriptor(
            string name,
            ArgumentValueType valueType,
            bool isRequired = false,
            bool isRepeatable = false,
            IReadOnlyList<CommandVariant>? variants = null)
        {
            Name = name;
            ValueType = valueType;
            IsRequired = isRequired;
            IsRepeatable = isRepeatable;
        }
    }
}
