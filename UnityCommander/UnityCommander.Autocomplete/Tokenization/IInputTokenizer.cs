using UnityCommander.Autocomplete.Input;

namespace UnityCommander.Autocomplete.Tokenization
{
    public interface IInputTokenizer
    {
        TokenizationResult Tokenize(InputState state);
    }
}
