
namespace UnityCommander.Common.Commands
{
    using System.Windows.Input;

    /// <summary>
    /// The GlobalCommand interface.
    /// </summary>
    public interface IGlobalCommand
    {
        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        public object CommandParameter { get; set; }
    }
}
