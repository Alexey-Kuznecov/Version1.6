
namespace UnityCommander.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityCommander.Common;
    using UnityCommander.Common.Commands;
    using UnityCommander.Core.IO.Operations;

    public class GlobalCommandProvider : IGlobalCommandProvider
    {
        private static readonly Queue<IGlobalCommand> GlobalCommands = new ();
        
        private static readonly GlobalCommandManager GlobalCommandManager = new (GlobalCommands);

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalCommandProvider"/> class.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public GlobalCommandProvider(object command)
        {
            GlobalCommandManager.CreateCommand(command);
        }

        public IGlobalCommandManager GetCommandManager() => GlobalCommandManager;

        internal static IGlobalCommand FindCommand(string commandName) => GlobalCommandManager.GetCommand(commandName);
    }
}
