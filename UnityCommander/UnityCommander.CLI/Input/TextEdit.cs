
namespace UnityCommander.CLI.Input
{
    public class TextEdit
    {
        public int ReplaceStart { get; }
        public int ReplaceLength { get; }
        public string InsertText { get; }
        public InputToken CurrentToken { get; } // добавляем текущий токен

        public TextEdit(int start, int length, InputToken currentToken, string insertText)
        {
            ReplaceStart = start;
            ReplaceLength = length;
            InsertText = insertText;
            CurrentToken = currentToken ?? new InputToken
            {
                Start = start,
                Length = length,
                CurrentValue = string.Empty
            };
        }
    }
}
