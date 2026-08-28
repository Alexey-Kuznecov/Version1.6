
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Helper;
using UnityCommander.CLI.Integration;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleLineExecutor
    {
        private readonly IServiceProvider _services;
        private readonly ConsoleCommandDispatcher _dispatcher;

        public ConsoleLineExecutor(
            IServiceProvider services,
            ConsoleCommandDispatcher dispatcher)
        {
            _services = services;
            _dispatcher = dispatcher;
        }

        public async Task<CommandExecutionResult> ExecuteAsync(
            ConsoleSession session,
            string line,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(line))
                return CommandExecutionResult.Failed;

            var parts = ParseHelper.ParseArguments(line);

            if (parts.Length == 0)
                return CommandExecutionResult.Failed;

            var name = parts[0];
            var args = parts.Skip(1).ToArray();

            var context = new ConsoleCommandContext(
                _services,
                session.Output,
                args,
                line);

            await _dispatcher.ExecuteCommandAsync(
                name,
                context,
                cancellationToken);

            return CommandExecutionResult.Success;
        }
    }
}
