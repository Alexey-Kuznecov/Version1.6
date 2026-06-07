namespace UnityCommander.CLI.Core
{
    public class ConsoleCommandContext : IConsoleCommandContext
    {
        public string[] Arguments { get; }
        public IServiceProvider Services { get; }
        public IConsoleOutput Output { get; }
        public string Input { get; }

        public ConsoleCommandContext(IServiceProvider services, IConsoleOutput output, string[] args, string? input = null)
        {
            Arguments = args;
            Services = services;
            Output = output;
            Input = input ?? string.Empty; // или throw new ArgumentNullException(nameof(input));
        }
    }
}
