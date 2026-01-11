
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class InputStatus
    {
        public ICommandVariant? Variant { get; set; }
        public AnalyzerToken ActiveToken { get; set; }

        public ICommandDescriptor? Command { get; set; }

        public IReadOnlyList<AnalyzerToken> Tokens { get; set; }

        public ExpectedKind ExpectedKind { get; set; }
    }
}
