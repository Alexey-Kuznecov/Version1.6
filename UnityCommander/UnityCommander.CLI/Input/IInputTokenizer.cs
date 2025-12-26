
using UnityCommander.Logging.Abstractions;

namespace UnityCommander.CLI.Input
{
    public interface IInputTokenizer
    {
        TokenizationResult Tokenize(InputState state);
    }
}
