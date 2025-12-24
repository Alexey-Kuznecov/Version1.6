
namespace UnityCommander.CLI.Autocomplete
{
    [Obsolete("Use Microsoft.Extensions.Logging instead.")]
    public interface IAutoCompleteProvider
    {
        IEnumerable<string> GetSuggestions(string input);
    }
}
