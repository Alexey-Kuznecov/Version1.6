
namespace UnityCommander.Common
{
    using UnityCommander.Common.Commands;

    /// <summary>
    /// The context menu.
    /// </summary>
    public class ContextItem
    {
        /// <summary>
        /// Gets or sets the menu header.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public IGlobalCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public string CommandName { get; set; }
    }
}
