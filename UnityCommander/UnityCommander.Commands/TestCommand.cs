
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;

namespace UnityCommander.Commands
{
    [ConsoleCommand("test", "Команда для тестирования кода")]
    public class TestCommand : IConsoleCommand
    {
        public string Name => "test";
        public string Description => "Команда для тестирования кода";

        public async Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken)
        {

        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
