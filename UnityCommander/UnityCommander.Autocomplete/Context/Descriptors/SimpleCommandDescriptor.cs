
namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class SimpleCommandDescriptor : ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<CommandVariant> Variants { get; }

        public SimpleCommandDescriptor(
            string name,
            IReadOnlyList<CommandVariant> variants)
        {
            Name = name;
            Variants = variants;
        }
    }
}
