
namespace UnityCommander.Common.Commands
{
    using System;
    using System.Collections.Generic;

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
        IGlobalCommand GetCommand(string commandName);

        /// <summary>
        /// The get command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <returns>
        /// The <see cref="GlobalCommand"/>.
        /// </returns>
        IEnumerable<IGlobalCommand> GetCommands(string commandName);

        /// <summary>
        /// The get commands.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        Queue<IGlobalCommand> GetCommands();

        /// <summary>
        /// The create command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        void CreateCommand(object command);

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
        void CreateSingletonCommand(string commandName, object instance, Action<object> action);


        void UpdateCommand(string commandName);
    }
}
