using UnityCommander.Autocomplete.Context.Descriptors;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class ParsedFlag
    {
        public IFlagDescriptor Descriptor { get; }
        public string? Value { get; }
        public bool HasValue => Value != null;
        public ParsedFlag(
            IFlagDescriptor descriptor,
            string? value)
        {
            Descriptor = descriptor;
            Value = value;
        }
    }
}
