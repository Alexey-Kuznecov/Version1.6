
using UnityCommander.Logging.Abstractions;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.CLI.Input
{
    public sealed class CompletionEngine : ICompletionEngine
    {
        private readonly IInputTokenizer _tokenizer;
        private readonly IInputContextResolver _contextResolver;
        private readonly IEnumerable<ICompletionProvider> _providers;
        private readonly ITokenRegistry _tokenRegistry; // внутренний регистр
        private readonly ILogger? _appLogger; // внутренний регистр
        public CompletionEngine(
            IInputTokenizer tokenizer,
            IInputContextResolver contextResolver,
            IEnumerable<ICompletionProvider> providers,
            ITokenRegistry tokenRegistry,
            ILogger? appLogger = null)
        {
            _appLogger = appLogger;
            _tokenizer = tokenizer;
            _contextResolver = contextResolver;
            _providers = providers;
            _tokenRegistry = tokenRegistry;
        }

        public CompletionResult GetCompletions(InputState state)
        {
            var tokens = _tokenizer.Tokenize(state);
            _tokenRegistry.UpdateTokens(tokens.Tokens); // обновляем регистр
            var context = _contextResolver.Resolve(tokens);

            var items = _providers
                .Where(p => p.CanHandle(context))
                .SelectMany(p => p.GetCompletions(context))
                .ToList();

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

        // Новая точка доступа к токенам из VM
        public InputToken? GetTokenNearCaret(string text, int caretPosition)
            => _tokenRegistry.GetTokenNearCaret(text, caretPosition);

        public IReadOnlyList<InputToken> GetAllTokens()
            => _tokenRegistry.Tokens;
    }
}
