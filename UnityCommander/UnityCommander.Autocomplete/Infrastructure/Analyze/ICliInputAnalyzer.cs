

namespace UnityCommander.Autocomplete.Infrastructure.Analyze
{
    public interface ICliInputAnalyzer
    {
        InputStatus Analyze(
            string text,
            int caretPosition
        );
    }
}
