

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public sealed class AnalyzerToken
    {
        public string Text { get; }

        public int Start { get; }

        public int Length => Text.Length;

        public int End => Start + Length;
        public bool IsActive { get; set; }

        public TokenKind Kind { get; set; }

        public TokenStatus Status { get; set; }

        public bool IsComplete { get; set; }

        public int SemanticIndex { get; internal set; }

        public AnalyzerToken(string text, int start)
        {
            Text = text;
            Start = start;
        }


        // ДОБАВЛЕН ДЛЯ ДИАГНОСТИКИ
        public AnalyzerToken Clone()
        {
            return new AnalyzerToken(Text, Start)
            {
                Kind = Kind,
                Status = Status,
                IsComplete = IsComplete,
                SemanticIndex = SemanticIndex
            };
        }
    }
}
