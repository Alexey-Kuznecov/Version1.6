
namespace UnityCommander.Common.Commands
{
    using System.Windows.Input;

    /// <summary>
    /// The GlobalCommand interface.
    /// </summary>
    public interface IGlobalCommand : ICommandBase
    {
        /// <summary>
        /// Gets or sets the shortcut key.
        /// </summary>
        public InputGesture ShortcutKey { get; set; }

        public GlobalCommandSelection SelectionFlag { get; set; }
    }
}
