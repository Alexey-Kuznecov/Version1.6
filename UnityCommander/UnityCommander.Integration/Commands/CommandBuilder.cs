
using System.Collections.Generic;

namespace UnityCommander.Integration.Commands
{
    public class CommandBuilder
    {
        private readonly List<CommandBase> globalCommands;

        public CommandBuilder()
        {
            this.globalCommands = new List<CommandBase>();
        }

        public void Register<TOv, TOr>() where TOv : TOr, new()
        {
            this.globalCommands.Add(new TOv() as CommandBase);
        }

        public IEnumerable<CommandBase> GetCommands() => globalCommands;
    }
}
