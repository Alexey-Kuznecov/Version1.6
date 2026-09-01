
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
                return new CommandExecutionResult { Success = false };

            var parts = ParseHelper.ParseArguments(line);

            if (parts.Length == 0)
                return new CommandExecutionResult { Success = false };

            var directives = CommandExecutionDirective.None;

            if (parts[0] == "startup:")
            {
                directives |= CommandExecutionDirective.Startup;
                parts = parts.Skip(1).ToArray();
            }

            if (parts.Length == 0)
                return new CommandExecutionResult { Success = false };

            var name = parts[0];
            var args = parts.Skip(1).ToArray();

            var context = new ConsoleCommandContext(
                _services,
                session.Output,
                args,
                line);

            try
            {
                await _dispatcher.ExecuteCommandAsync(
                    name,
                    context,
                    cancellationToken);

                return new CommandExecutionResult
                {
                    Success = true,
                    Directives = directives
                };
            }
            catch (OperationCanceledException)
            {
                return new CommandExecutionResult
                {
                    Success = false
                };
            }
            catch (Exception ex)
            {
                session.Output.WriteError(ex.Message);

                return new CommandExecutionResult
                {
                    Success = false
                };
            }
        }
    }
}
