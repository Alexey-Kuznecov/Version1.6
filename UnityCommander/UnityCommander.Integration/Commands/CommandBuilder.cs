
namespace UnityCommander.Integration.Commands
{
    using System.Collections.Generic;

    using UnityCommander.Common.Commands;

    public class CommandBuilder
    {
        private readonly List<BaseCommand> globalCommands = new ();
        
        public void Register<TOv, TOr>() where TOv : TOr, new ()
        {
            globalCommands.Add(new TOv() as BaseCommand);
        }

        public IEnumerable<BaseCommand> GetCommands() => globalCommands;
    }
}
