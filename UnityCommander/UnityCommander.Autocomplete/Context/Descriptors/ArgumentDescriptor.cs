using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    // ===== Альтернативный общий аргумент =====
    public sealed class ArgumentDescriptor : IPositionalArgumentDescriptor
    {
        public string Name { get; init; } = string.Empty;
        public ArgumentValueType ValueType { get; init; }
        public bool IsRequired { get; init; }
        public bool IsRepeatable { get; init; }
        public ICommandVariant? Variant { get; init; }
    }
}