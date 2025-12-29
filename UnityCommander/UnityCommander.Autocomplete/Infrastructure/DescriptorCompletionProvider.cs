
using UnityCommander.Abstractions.Completion;
using UnityCommander.Autocomplete.Completion;
using UnityCommander.Autocomplete.Context;
using UnityCommander.Autocomplete.Input;
using UnityCommander.Autocomplete.Tokenization;

namespace UnityCommander.Autocomplete.Infrastructure
{
    // Provider для аргументов и флагов
    public sealed class DefaultCompletionProvider // : ICompletionProvider
    {
        private readonly IInputTokenizer _tokenizer;
        private readonly ITokenRegistry _tokenRegistry;

        public DefaultCompletionProvider(IInputTokenizer inputTokenizer, ITokenRegistry tokenRegistry)
        {
            _tokenizer = inputTokenizer ?? throw new ArgumentNullException(nameof(inputTokenizer));
            _tokenRegistry = tokenRegistry ?? throw new ArgumentNullException(nameof(tokenRegistry));
        }

        public bool CanHandle(InputContext context) => context is ArgumentContext;

        public IEnumerable<CompletionItem> GetCompletions(InputContext context, InputState state)
        {
            var argCtx = (ArgumentContext)context;

            // Выбираем, что выдаём: флаги или аргументы
            var items = argCtx.ExpectedNext switch
            {
                CompletionKind.Flag => argCtx.AvailableFlags.Select(f => ToCompletionItem(f.Name, state, argCtx)),
                CompletionKind.Variant => argCtx.AvailableArguments.Select(a => ToCompletionItem(a.Name, state, argCtx)),
                _ => Enumerable.Empty<CompletionItem>()
            };

            return items;
        }

        private CompletionItem ToCompletionItem(string text, InputState state, ArgumentContext ctx)
        {
            return new CompletionItem
            {
                DisplayText = text,
                EditFactory = s =>
                {
                    var tokens = _tokenizer.Tokenize(s) ?? throw new ArgumentNullException();
                    _tokenRegistry.UpdateTokens(tokens.Tokens ?? throw new ArgumentNullException()); // обновляем регистр
                    int start = tokens?.CurrentTokenStart ?? s.CaretPosition;
                    int length = tokens?.CurrentTokenLength ?? 0;
                    return new TextEdit(start, length, tokens.CurrentToken, text + " ");
                }
            };
        }
    }
}
