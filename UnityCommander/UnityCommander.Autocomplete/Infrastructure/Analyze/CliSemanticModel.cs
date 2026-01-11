
using UnityCommander.Abstractions.Completion;

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class CliSemanticModel
    {
        public ICommandDescriptor? Command { get; }
        public ICommandVariant? Variant { get; }

        public IReadOnlyList<AnalyzerToken> Tokens { get; }

        public int PositionalIndex { get; }
        public IReadOnlyList<ParsedArgument> PositionalArguments { get; }
        public IReadOnlyList<ParsedFlag> Flags { get; }
    }
}
