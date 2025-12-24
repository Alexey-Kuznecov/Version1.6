
namespace UnityCommander.CLI.Input
{
    public abstract class InputContext
    {
        public TokenizationResult Tokens { get; }

        public InputContext(TokenizationResult tokens)
        {
            Tokens = tokens;
        }
    }
}
