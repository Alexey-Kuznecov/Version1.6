using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public sealed class SimpleCommandDescriptor : ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<IPositionalArgumentDescriptor> PositionalArguments { get; }
        public IReadOnlyList<IFlagDescriptor> Flags { get; }
        public Dictionary<string, CommandVariant> Arguments { get; } // аргументы для этого режима
        public FlagOrderPolicy FlagOrderPolicy { get; }
        public string Usage { get; }

        public SimpleCommandDescriptor(
            string name,
            IReadOnlyList<IPositionalArgumentDescriptor> positionalArguments,
            IReadOnlyList<IFlagDescriptor> flags,
            FlagOrderPolicy flagOrderPolicy,
            string usage)
        {
            Name = name;
            PositionalArguments = positionalArguments;
            Flags = flags;
            FlagOrderPolicy = flagOrderPolicy;
            Usage = usage;
        }
    }
}
