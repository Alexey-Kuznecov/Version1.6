
using UnityCommander.Abstractions.Completion;
using UnityCommander.Logging.Contracts;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class InputStatus
    {
        public ICommandVariant? Variant { get; internal set; }

        public AnalyzerToken? ActiveToken { get; internal set; }

        public bool IsValidCommand { get; internal set; }

        public ICommandDescriptor? Command { get; internal set; }

        public required IReadOnlyList<AnalyzerToken> Tokens { get; set; }

        public ExpectedKind ExpectedKind { get; internal set; }

        public ILogger? Logger { get; set; }
    }
}
