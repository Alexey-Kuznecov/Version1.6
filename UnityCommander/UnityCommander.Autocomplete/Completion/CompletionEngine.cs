
using UnityCommander.Autocomplete.Infrastructure;
using UnityCommander.Autocomplete.Input;
using UnityCommander.Autocomplete.Tokenization;
using UnityCommander.Logging.Contracts;
using UnityCommander.Logging.Core;
using UnityCommander.Logging.Infrastructure;

namespace UnityCommander.Autocomplete.Completion
{
    public sealed class CompletionEngine : ICompletionEngine
    {
        private readonly ITokenRegistry _tokenRegistry;
        private readonly IEnumerable<ICompletionProvider> _providers;
        private readonly ILogger? _logger;

        public CompletionEngine(
            ITokenRegistry tokenRegistry, 
            IEnumerable<ICompletionProvider> providers, 
            LoggerCreator? loggerCreator = null)
        {
            _tokenRegistry = tokenRegistry;
            _providers = providers;
            _logger = loggerCreator?.For<CompletionEngine>(LogScope.Runtime);
        }

        public CompletionResult GetCompletions(InputState state, CliParseState analyze)
        {
            _logger?.Info(
               $"GetCompletions: " +
               $"Text='{state.Text}', " +
               $"Caret={state.CaretPosition}, " +
               $"CurrentToken='{analyze.CurrentToken}', " +
               $"ReplaceStart={analyze.ReplaceStart}, " +
               $"ReplaceLength={analyze.ReplaceLength}");

            var items = _providers
                .Where(p => p.CanHandle(analyze))
                .SelectMany(p => p.GetCompletions(analyze))
                .Select(item => new CompletionItem
                {
                    DisplayText = item.DisplayText,
                    InsertText = item.InsertText,
                    EditFactory = s => new TextEdit(
                        analyze.ReplaceStart,
                        analyze.ReplaceLength,
                        analyze.CurrentToken,
                        item.InsertText + " "
                    )
                })
                .ToList();

            _logger?.Info(
               $"GetCompletions: " +
               $"Text='{state.Text}', " +
               $"Caret={state.CaretPosition}, " +
               $"CurrentToken='{analyze.CurrentToken}', " +
               $"ReplaceStart={analyze.ReplaceStart}, " +
               $"ReplaceLength={analyze.ReplaceLength}");

            return new CompletionResult(items)
            {
                DefaultSelectedIndex = items.Count > 0 ? items.Count - 1 : -1
            };
        }

        public TextEdit ApplyCompletion(InputState state, CompletionItem item)
        {
            if (item.EditFactory == null)
                throw new ArgumentNullException(nameof(item.EditFactory), "EditFactory не должен быть null.");
            return item.EditFactory(state);
        }

        public InputToken? GetTokenNearCaret(string text, int caretPosition)
            => _tokenRegistry.GetTokenNearCaret(text, caretPosition);

        public IReadOnlyList<InputToken> GetAllTokens()
            => _tokenRegistry.Tokens;
    }
}
