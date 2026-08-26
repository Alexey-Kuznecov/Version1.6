
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class AnalyzerContext
    {
        public ICommandDescriptor? Command;
        public ICommandVariant? Variant;

        public IFlagDescriptor? WaitingFlagValue;

        public int PositionalIndex;

        public List<IFlagDescriptor> UsedFlags { get; } = new();
        public bool HasUsedFlags { get; internal set; }
    }
}
