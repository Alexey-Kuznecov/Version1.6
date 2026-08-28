
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Integration;
using UnityCommander.Modules.BottomPanel.Console;

namespace UnityCommander.Modules.BottomPanel.Services
{
    public sealed class StartupCommandRunner : IStartupCommandRunner
    {
        private readonly ConsoleCommandDispatcher _dispatcher;

        public StartupCommandRunner(ConsoleCommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task RunStartupCommands(
            ConsoleSession session,
            CancellationToken cancellationToken)
        {
            foreach (var command in session.Profile.StartupCommands)
            {
                await _dispatcher.ExecuteCommandAsync(
                    command,
                    session.Context,
                    cancellationToken);
            }
        }
    }
}
