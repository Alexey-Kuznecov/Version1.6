
namespace UnityCommander.Common.Commands
{
    using System;
    using System.Collections.Generic;

    using UnityCommander.Integration.Commands;

    /// <summary>
    /// The GlobalCommandManager interface.
    /// </summary>
    public interface IGlobalCommandManager
    {
        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <returns>
        /// The <see cref="GlobalCommand"/>.
        /// </returns>
        GlobalCommand GetCommand(string commandName);

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<GlobalCommand> GetCommands();

        /// <summary>
        /// The create command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void CreateCommand(BaseCommand command);

        /// <summary>
        /// The create command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        void CreateCommand(string commandName, object instance, Action<object> action);
    }
}
