
using System;

namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The context menu.
    /// </summary>
    public class ContextMenuItem
    {
        /// <summary>
        /// Gets or sets the menu header.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public Action Command { get; set; }
    }
}
