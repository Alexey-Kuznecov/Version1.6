using UnityCommander.CLI.Core;

namespace UnityCommander.CLI.Integration
{
    public class ConsoleCommandFactory
    {
        public IConsoleCommand Create(ConsoleCommandMetadata metadata)
        {
            return new ConsoleCommandAdapter(metadata);
        }
    }
}
