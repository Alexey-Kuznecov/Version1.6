
namespace UnityCommander.Modules.LeftSideBars.Models
{
    public sealed class SearchTextPart
    {
        public string Text { get; }
        public bool IsMatch { get; }

        public SearchTextPart(string text, bool isMatch)
        {
            Text = text;
            IsMatch = isMatch;
        }
    }
}
