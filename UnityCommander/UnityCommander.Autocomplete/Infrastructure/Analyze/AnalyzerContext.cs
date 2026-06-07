
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public class AnalyzerContext
    {
        public ICommandDescriptor? Command;
        public ICommandVariant? Variant;

        public IFlagDescriptor? WaitingFlagValue;
        public int PositionalIndex;

        public bool IsStrictOrder;
    }
}
