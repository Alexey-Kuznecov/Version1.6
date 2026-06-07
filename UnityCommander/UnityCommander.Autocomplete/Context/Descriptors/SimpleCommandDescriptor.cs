
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public class SimpleCommandDescriptor : ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<ICommandVariant> Variants { get; }

        public SimpleCommandDescriptor(
            string name,
            IReadOnlyList<ICommandVariant> variants)
        {
            Name = name;
            Variants = variants;
        }
    }
}
