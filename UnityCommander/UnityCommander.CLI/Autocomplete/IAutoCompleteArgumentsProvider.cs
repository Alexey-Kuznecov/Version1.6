
namespace UnityCommander.CLI.Autocomplete
{
    public interface IAutoCompleteArgumentsProvider
    {
        IEnumerable<string> GetArgumentSuggestions(string[] currentArgs);
    }
}
