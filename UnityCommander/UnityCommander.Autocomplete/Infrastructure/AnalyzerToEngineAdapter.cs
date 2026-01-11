
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Infrastructure
{
    public class AnalyzerToEngineAdapter
    {
        private readonly ICompletionEngine _engine;

        public AnalyzerToEngineAdapter(ICompletionEngine engine)
        {
            _engine = engine;
        }

        public CompletionResult GetCompletionsFromAnalyzer(
            string inputText,
            CliParseState parseState)
        {
            // Превращаем логические правила Analyzer в контекст Engine
            var rules = parseState.ExpectedNext switch
            {
                CompletionKind.Flag => parseState.AvailableFlags.Select(f => f.Name).ToList(),
                CompletionKind.PositionalArgument => parseState.AvailableArguments.Select(a => a.Name).ToList(),
                _ => new List<string>()
            };

            // Находим токен рядом с кареткой
            var caretIndex = inputText.Length; // Можно уточнять, если нужно смещать
            var tokenNearCaret = _engine.GetTokenNearCaret(inputText, caretIndex);

            // Строим CompletionItems для Engine
            var items = rules.Select(value => new CompletionItem
            {
                DisplayText = value,
                EditFactory = state => new TextEdit(
                    tokenNearCaret?.Start ?? caretIndex,
                    tokenNearCaret?.Length ?? 0,
                    new InputToken(),
                    value)
            }).ToList();

            return new CompletionResult(items)
            {
                DefaultSelectedIndex = items.Count > 0 ? 0 : -1
            };
        }
    }
}
