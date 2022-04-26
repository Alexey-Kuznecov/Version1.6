
using System;
using System.Windows.Input;
using UnityCommander.Integration.Commands;
using UnityCommander.Integration.Enums;

namespace UnityCommander.Integration.Contracts
{
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
        public GlobalCommand Command { get; set; }

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
