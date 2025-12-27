using UnityCommander.Autocomplete.Infrastructure;

namespace UnityCommander.Autocomplete.Context.Descriptors
{
    public interface ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<IPositionalArgumentDescriptor> PositionalArguments { get; }
        public IReadOnlyList<IFlagDescriptor> Flags { get; }
        public Dictionary<string, CommandVariant> Arguments { get; }
        FlagOrderPolicy FlagOrderPolicy { get; }
        string Usage { get; }
    }
}
