
namespace UnityCommander.CLI.Input
{
    public sealed class TextEdit
    {
        public int ReplaceStart { get; }
        public int ReplaceLength { get; }
        public string InsertText { get; }

        public TextEdit(int start, int length, string insertText)
        {
            ReplaceStart = start;
            ReplaceLength = length;
            InsertText = insertText;
        }
    }
}
