
using UnityCommander.Logging.Abstractions;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.CLI.Input
{
    public sealed class SimpleInputTokenizer : IInputTokenizer
    {
        private readonly ILogger? _appLogger; // внутренний регистр

        public SimpleInputTokenizer(ILogger? appLogger = null)
        {
            _appLogger = appLogger;
        }

        public TokenizationResult Tokenize(InputState state)
        {
            _appLogger?.Info($"Tokenize called with caretPos: {state.CaretPosition} on text: '{state.Text}'");
            var text = state.Text;
            var tokens = new List<InputToken>();

            int i = 0;
            while (i < text.Length)
            {
                while (i < text.Length && text[i] == ' ')
                    i++;

                if (i >= text.Length)
                    break;

                int start = i;

                while (i < text.Length && text[i] != ' ')
                    i++;

                int length = i - start;

                tokens.Add(new InputToken
                {
                    CurrentValue = text.Substring(start, length),
                    OriginalValue = text.Substring(start, length),
                    Start = start,
                    Length = length
                });
            }

            foreach (var token in tokens)
            {
                InputToken? currentToken2 = tokens.FirstOrDefault(t =>
                state.CaretPosition >= t.Start &&
                state.CaretPosition < t.Start + t.Length);
                _appLogger?.ObjectInfo($"text: {token.CurrentValue} is found: {currentToken2 != null} ", token);

            }

            InputToken? currentToken = tokens.FirstOrDefault(t =>
                state.CaretPosition >= t.Start &&
                state.CaretPosition < t.Start + t.Length);

            int currentStart;

            if (tokens.Count == 0)
            {
                currentStart = 0;
            }
            else
            {
                var lastToken = tokens.Last();

                if (state.CaretPosition > lastToken.Start + lastToken.Length)
                    currentStart = state.CaretPosition;
                else
                    currentStart = lastToken.Start + lastToken.Length;
            }

            return new TokenizationResult
            {
                Tokens = tokens,
                CaretPosition = state.CaretPosition,
                CurrentTokenStart = currentToken?.Start ?? state.CaretPosition,
                CurrentTokenLength = currentToken?.Length ?? 0,
                CurrentTokenValue = currentToken?.CurrentValue ?? string.Empty,
                CurrentToken = currentToken
            };
        }
    }
} 
