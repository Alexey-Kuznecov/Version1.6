
using UnityCommander.Autocomplete.Infrastructure.Analyze;

namespace UnityCommander.Autocomplete.Diagnostic
{
    public sealed class InputDiagnostics
    {
        public string Text { get; init; } = string.Empty;
        public int CaretIndex { get; init; }

        public string BeforeCaret { get; init; } = string.Empty;
        public string AfterCaret { get; init; } = string.Empty;

        public AnalyzerToken? CurrentToken { get; init; }
        
        public InputStatus? Status { get; init; }

        public IReadOnlyList<AnalyzerToken> Tokens { get; init; } = [];
    }
}
