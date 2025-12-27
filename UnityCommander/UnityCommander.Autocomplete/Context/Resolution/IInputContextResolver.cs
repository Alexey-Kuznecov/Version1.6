using UnityCommander.Autocomplete.Tokenization;

namespace UnityCommander.Autocomplete.Context.Resolution
{
    public interface IInputContextResolver
    {
        InputContext Resolve(TokenizationResult tokens);
    }
}
