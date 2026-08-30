
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

        public CompletionResult GetCompletions(
           InputState state,
           CliParseState analyze)
        {
            var providers = _providers
                .Where(p => p.CanHandle(analyze))
                .OrderByDescending(p => p.Priority)
                .ToList();

            var priority = providers.FirstOrDefault()?.Priority;

            var items = priority.HasValue
                ? providers
                    .Where(p => p.Priority == priority.Value)
                    .SelectMany(p => p.GetCompletions(analyze))
                    .Select(item => new CompletionItem
                    {
                        DisplayText = item.DisplayText,
                        InsertText = item.InsertText,
                        CaretOffset = item.CaretOffset,
                        EditFactory = s => new TextEdit(
                            analyze.ReplaceStart,
                            analyze.ReplaceLength,
                            analyze.CurrentToken,
                            item.InsertText + " "
                        )
                    })
                    .ToList()
                : [];

            return new CompletionResult(items)
            {
                DefaultSelectedIndex =
                    items.Count > 0 ? items.Count - 1 : -1
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
