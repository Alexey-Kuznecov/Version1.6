
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public sealed class ParsedArgument
    {
        public IPositionalArgumentDescriptor Descriptor { get; }
        public string Value { get; }

        public ParsedArgument(
            IPositionalArgumentDescriptor descriptor,
            string value)
        {
            Descriptor = descriptor;
            Value = value;
        }
    }
}
