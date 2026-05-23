
using CommandSystem.Abstractions;

namespace UnityCommander.CLI.Core
{
    public class ConsoleCommandMetadata
    {
        public CommandMetadata? Metadata { get; }
        public IReadOnlyList<string> Aliases { get; }
        public string? Name => Metadata?.Name;
        public string? Description => Metadata?.Description;

        public Func<IConsoleCommandContext, CancellationToken, Task> Handler { get; }

        public ConsoleCommandMetadata(CommandMetadata metadata, 
            Func<IConsoleCommandContext, CancellationToken, Task> handler,
            IEnumerable<string>? aliases = null)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            Aliases = aliases?.ToList() ?? new List<string>();
        }
    }
}
