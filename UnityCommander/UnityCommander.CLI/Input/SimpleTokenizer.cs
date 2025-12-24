
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

            return new TokenizationResult
            {
                Tokens = tokens,
                CaretPosition = state.CaretPosition
            };
        }
    }
} 
