
using System.Windows;

namespace UnityCommander.Common.Dialog
{
    public sealed class DialogOptions
    {
        public string Title { get; set; } = string.Empty;

        public double Width { get; set; } = 800;
        public double Height { get; set; } = 600;

        public bool IsResizable { get; set; } = true;

        public WindowStartupLocation StartupLocation { get; set; }
            = WindowStartupLocation.CenterOwner;
    }
}
