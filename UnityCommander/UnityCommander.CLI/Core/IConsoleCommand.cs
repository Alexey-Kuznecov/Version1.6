
using UnityCommander.CLI.Mode;
using UnityCommander.Common.Commands;

namespace UnityCommander.CLI.Core
{
    public interface IConsoleCommand : IConsoleCommandBase
    {
        IEnumerable<string> Aliases => Enumerable.Empty<string>();
        IEnumerable<string> GetSuggestions(string[] args) => Enumerable.Empty<string>(); // <- поддержка автодополнения по аргументам
        string Name { get; }
        string Description { get; }
        CommandExecutionMode Mode => CommandExecutionMode.Blocking;
        Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken);
        Task FinalizeAsync();
    }
}
