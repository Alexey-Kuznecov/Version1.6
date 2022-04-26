
using System.Collections.Generic;

namespace UnityCommander.Integration.Commands
{
    public class CommandBuilder
    {
        private readonly List<BaseCommand> globalCommands = new ();
        
        public void Register<TOv, TOr>() where TOv : TOr, new()
        {
            globalCommands.Add(new TOv() as BaseCommand);
        }

        public IEnumerable<BaseCommand> GetCommands() => globalCommands;
    }
}
