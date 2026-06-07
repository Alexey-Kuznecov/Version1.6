

using System.Text.RegularExpressions;
using System.Windows.Media;

namespace UnityCommander.Modules.BottomPanel.Highlighting
{
    public sealed class PathHighlightRule : ILogHighlightRule
    {
        public Regex Pattern { get; } =
            new(
                @"[A-Za-z]:\\(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*",
                RegexOptions.Compiled);

        public HighlightStyle GetStyle(string value)
            => new(
                Brushes.DarkGray,
                Brushes.DarkSlateGray);
    }
}
