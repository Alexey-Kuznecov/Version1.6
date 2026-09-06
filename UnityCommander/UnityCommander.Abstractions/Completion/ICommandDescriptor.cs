
namespace UnityCommander.Abstractions.Completion
{
    public interface ICommandDescriptor
    {
        public string Name { get; }
        public IReadOnlyList<ICommandVariant> Variants { get; }
        public IReadOnlyList<IFlagDescriptor>? Flags { get; }
        public IReadOnlyList<IPositionalArgumentDescriptor>? Arguments { get; }

        public FlagOrderPolicy FlagOrderPolicy { get; }
        public PositionalArgumentPolicy PositionalArgumentPolicy { get; }
        public string? Usage { get; }
        public bool IsStrictOrder { get; }
    }
}
