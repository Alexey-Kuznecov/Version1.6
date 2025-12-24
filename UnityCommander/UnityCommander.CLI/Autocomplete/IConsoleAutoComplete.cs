
namespace UnityCommander.Services.Interfaces
{
    public interface IConsoleAutoComplete
    {
        IEnumerable<string> GetSuggestions(string input);
    }
}
