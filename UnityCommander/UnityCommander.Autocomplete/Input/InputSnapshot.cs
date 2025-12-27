
namespace UnityCommander.Autocomplete.Input
{
    public class InputSnapshot
    {
        public string Text { get; }
        public IReadOnlyList<string> Tokens { get; }

        public InputSnapshot(string text, IReadOnlyList<string> tokens)
        {
            Text = text;
            Tokens = tokens;
        }
    }
}
