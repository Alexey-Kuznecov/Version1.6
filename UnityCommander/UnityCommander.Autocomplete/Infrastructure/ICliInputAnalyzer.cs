
namespace UnityCommander.Autocomplete.Infrastructure
{
    public interface ICliInputAnalyzer
    {
        CliParseState Analyze(
            string text,
            int caretPosition
        );
    }
}
