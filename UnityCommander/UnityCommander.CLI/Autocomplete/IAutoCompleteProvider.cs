
namespace UnityCommander.CLI.Autocomplete
{
    public interface IAutoCompleteProvider
    {
        IEnumerable<string> GetSuggestions(string input);
    }
}
