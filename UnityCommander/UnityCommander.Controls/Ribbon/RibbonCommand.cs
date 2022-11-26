
namespace UnityCommander.Controls.Ribbon
{
    using System.Windows.Input;

    using Common.Commands;

    /// <summary>
    /// The i ribbon command.
    /// </summary>
    public class RibbonCommand : IGlobalCommand
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        /// TODO: Gets or sets the shortcut key.
        /// </summary>
        public InputGesture ShortcutKey { get; set; }

        public GlobalCommandSelection SelectionFlag { get; set; }
    }
}
