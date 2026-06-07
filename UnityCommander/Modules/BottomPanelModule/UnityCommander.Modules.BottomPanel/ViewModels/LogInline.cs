
using System.Windows.Media;
using UnityCommander.Modules.BottomPanel.Highlighting;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public sealed class LogInline
    {
        public string Text { get; }
        public Brush Background { get; }
        public Brush Foreground { get; }

        public LogInline(string text, HighlightStyle style)
        {
            Text = text;
            Foreground = style.Foreground;
            Background = style.Background;
        }
    }
}
