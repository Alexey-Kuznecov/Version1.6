
namespace UnityCommander.Core
{
    using System.Windows.Input;

    using UnityCommander.Common.Commands;

    /// <summary>
    /// The plugin command.
    /// </summary>
    public class PluginCommand : IPluginCommand
    {
        public string Name { get; set; }

        public ICommand Command { get; set; }

        public object CommandParameter { get; set; }

        public InputGesture ShortcutKey { get; set; }
        public GlobalCommandSelection SelectionFlag { get; set; }
    }
}
