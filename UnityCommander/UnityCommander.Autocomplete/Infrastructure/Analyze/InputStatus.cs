
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Context.Descriptors;
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class InputStatus
    {
        public ICommandDescriptor? Command { get; internal set; }

        public ICommandVariant? Variant { get; internal set; }

        public AnalyzerToken? ActiveToken { get; internal set; }

        public IReadOnlyList<AnalyzerToken> Tokens { get; internal set; }
            = Array.Empty<AnalyzerToken>();

        public CompletionKind ExpectedKind { get; internal set; }

        public ExpectedValue? ExpectedValue { get; set; }

        public int PositionalIndex { get; internal set; }

        public List<IFlagDescriptor> UsedFlags { get; } = new();
       
        public List<IPositionalArgumentDescriptor> AvailableArguments { get; } = new();

        public bool IsValidCommand { get; internal set; }

        public ILogger? Logger { get; set; }
 
        public IReadOnlyList<SimpleFlagDescriptor> AvailableFlags { get; set; } 
            = new List<SimpleFlagDescriptor>();
    }
}
