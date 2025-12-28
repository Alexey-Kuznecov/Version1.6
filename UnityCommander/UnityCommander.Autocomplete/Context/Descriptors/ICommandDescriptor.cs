using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public interface ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<CommandVariant> Variants { get; }
    }
}
