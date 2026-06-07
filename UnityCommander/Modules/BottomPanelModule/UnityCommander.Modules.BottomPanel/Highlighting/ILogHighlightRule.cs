

using System.Text.RegularExpressions;

namespace UnityCommander.Modules.BottomPanel.Highlighting
{
    public interface ILogHighlightRule
    {
        Regex Pattern { get; }

        HighlightStyle GetStyle(string value);
    }
}
