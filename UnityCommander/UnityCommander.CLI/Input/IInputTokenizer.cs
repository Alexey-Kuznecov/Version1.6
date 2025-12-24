
namespace UnityCommander.CLI.Input
{
    public interface IInputTokenizer
    {
        TokenizationResult Tokenize(InputState state);
    }
}
