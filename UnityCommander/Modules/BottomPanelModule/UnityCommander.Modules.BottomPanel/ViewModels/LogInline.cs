
using System.Windows.Media;

namespace UnityCommander.Modules.BottomPanel.ViewModels
{
    public sealed class LogInline
    {
        public string Text { get; }
        public Brush Color { get; }

        public LogInline(string text, Brush color)
        {
            Text = text;
            Color = color;
        }
    }
}
