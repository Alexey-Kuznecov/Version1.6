
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Helper;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleCommandLoop
    {
        private readonly ConsoleCommandDispatcher _dispatcher;
        private readonly IServiceProvider _services;
        private readonly ConsoleApplicationLifetime _lifetime;

        public ConsoleCommandLoop(
            ConsoleCommandDispatcher dispatcher,
            IServiceProvider services,
            ConsoleApplicationLifetime lifetime)
        {
            _dispatcher = dispatcher;
            _services = services;
            _lifetime = lifetime;
        }

        public async Task RunAsync(ConsoleSession session, CancellationToken cancellationToken)
        {
            session.Output.WriteLine("Unity Commander Internal Console ready.");

            while (_lifetime.IsRunning)
            {
                var line = await session.Input.ReadLineAsync(_lifetime.Token);

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = ParseHelper.ParseArguments(line);

                var name = parts[0];
                var args = parts.Skip(1).ToArray();

                var ctx = new ConsoleCommandContext(
                    _services,
                    session.Output,
                    args,
                    line);

                using var commandCts =
                    CancellationTokenSource.CreateLinkedTokenSource(
                        _lifetime.Token);

                await _dispatcher.ExecuteCommandAsync(
                    name,
                    ctx,
                    commandCts.Token);
            }
        }
    }
}
