
namespace UnityCommander.Abstractions.Completion
{
    public interface ICommandVariant
    {
        string? Name { get; } 
        IReadOnlyList<IFlagDescriptor> Flags { get; }
        IReadOnlyList<IPositionalArgumentDescriptor> Arguments { get; } 
        FlagOrderPolicy FlagOrderPolicy { get; }
        string Usage { get; }
        bool IsStrictOrder { get; }
    }
}