
namespace UnityCommander.CLI.Input
{
    public abstract class InputContext
    {
        public TokenizationResult Tokens { get; }
        public InputToken CurrentToken { get; }
        public InputContext(TokenizationResult tokens)
        {
            Tokens = tokens;
            CurrentToken = tokens.CurrentToken ?? new InputToken
            {
                Start = tokens.CaretPosition,
                Length = 0,
                CurrentValue = string.Empty
            };
        }
    }
}
