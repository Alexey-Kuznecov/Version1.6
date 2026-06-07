
using System.Windows.Media;

namespace UnityCommander.Modules.BottomPanel.Highlighting
{
    public sealed record HighlightStyle(
         Brush Foreground,
         Brush? Background = null);
}
