
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public class SimpleCommandDescriptor : ICommandDescriptor
    {
        public string Name { get; }

        public IReadOnlyList<ICommandVariant> Variants { get; }
        public IReadOnlyList<IFlagDescriptor> Flags { get; }
        public IReadOnlyList<IPositionalArgumentDescriptor> Arguments { get; }

        public FlagOrderPolicy FlagOrderPolicy { get; }

        public PositionalArgumentPolicy PositionalArgumentPolicy { get; }

        public string? Usage { get; }

        public bool IsStrictOrder { get; } = false;

        public SimpleCommandDescriptor(
            string name,
            IReadOnlyList<ICommandVariant>? variants = null,
            IPositionalArgumentDescriptor[]? arguments = null,
            IFlagDescriptor[] flags = null,
            string? usage = null,
            FlagOrderPolicy flagOrderPolicy = FlagOrderPolicy.AnyOrder,
            PositionalArgumentPolicy positionalArgumentPolicy = PositionalArgumentPolicy.AfterVariant)
        {
            Name = name;
            Variants = variants ?? Array.Empty<ICommandVariant>();
            Arguments = arguments ?? Array.Empty<IPositionalArgumentDescriptor>();
            Flags = flags ?? Array.Empty<IFlagDescriptor>();
            Usage = usage;
            FlagOrderPolicy = flagOrderPolicy;
            PositionalArgumentPolicy = positionalArgumentPolicy;
        }
    }
}
