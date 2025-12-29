

using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public class CommandVariant : ICommandVariant
    {
        public string? Name { get; } // mode-значение, например "commit"
        public IReadOnlyList<IFlagDescriptor> Flags { get; } // флаги для этого варианта
        public IReadOnlyList<IPositionalArgumentDescriptor> Arguments { get; } // аргументы для этого варианта
        public FlagOrderPolicy FlagOrderPolicy { get; }
        public string? Usage { get; }

        public CommandVariant(
            string name,
            IReadOnlyList<IFlagDescriptor> flags,
            IReadOnlyList<IPositionalArgumentDescriptor> arguments,
            FlagOrderPolicy flagOrderPolicy,
            string? usage = null)
        {
            Name = name;
            Flags = flags;
            Arguments = arguments;
            FlagOrderPolicy = flagOrderPolicy;
            Usage = usage;
        }
    }
}
