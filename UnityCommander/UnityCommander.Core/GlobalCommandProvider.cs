
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

        public GlobalCommandProvider()
        {
            GlobalCommandManager.CreateCommand(new FileManager());
        }

        public IGlobalCommandManager GetCommandManager() => GlobalCommandManager;

        internal static IGlobalCommand FindCommand(string commandName) => GlobalCommandManager.GetCommand(commandName);
    }
}
