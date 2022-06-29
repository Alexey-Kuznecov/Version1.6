
namespace UnityCommander.Core
{
    using System.Collections.Generic;

    using UnityCommander.Common.Commands;

    /// <summary>
    /// The global command provider.
    /// </summary>
    public class GlobalCommandProvider : IGlobalCommandProvider
    {
        /// <summary>
        /// The global commands.
        /// </summary>
        private static readonly Queue<IGlobalCommand> GlobalCommands = new ();

        /// <summary>
        /// The global command manager.
        /// </summary>
        private static readonly GlobalCommandManager GlobalCommandManager = new (GlobalCommands);

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandProvider"/> class.
        /// </summary>
        public GlobalCommandProvider()
        {
        }

        /// <summary>
        /// The get command manager.
        /// </summary>
        /// <returns>
        /// The <see cref="IGlobalCommandManager"/>.
        /// </returns>
        public IGlobalCommandManager GetCommandManager() => GlobalCommandManager;

        /// <summary>
        /// The find command.
        /// </summary>
        /// <param name="commandName">
        /// The command name.
        /// </param>
        /// <returns>
        /// The <see cref="IGlobalCommand"/>.
        /// </returns>
        internal static IGlobalCommand FindCommand(string commandName) => GlobalCommandManager.GetCommand(commandName);
    }
}
