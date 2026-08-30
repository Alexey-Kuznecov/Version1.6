
namespace UnityCommander.Abstractions.Completion
{
    public interface IFlagDescriptor : IValueDescriptor
    {        // --force
        string? ShortName { get; }    // -f
        
        ValueSeparator Separator { get; }

        bool IsRepeatable { get; }    // --tag a --tag b
        
        bool RequiresValue { get; }
    }
}