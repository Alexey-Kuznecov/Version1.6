namespace UnityCommander.CLI.Input
{
    public sealed class SimpleInputTokenizer : IInputTokenizer
    {
        public TokenizationResult Tokenize(InputState state)
        {
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
                    Value = text.Substring(start, length),
                    Start = start,
                    Length = length
                });
            }

            InputToken? currentToken = null;

            foreach (var token in tokens)
            {
                if (state.CaretPosition >= token.Start &&
                    state.CaretPosition <= token.Start + token.Length)
                {
                    currentToken = token;
                    break;
                }
            }

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
                CurrentTokenValue = currentToken?.Value ?? string.Empty,
            };
        }
    }
} 
