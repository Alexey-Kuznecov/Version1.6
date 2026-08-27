
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

        public async Task RunAsync(ConsoleSession session)
        {
            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    _lifetime.Token,
                    session.Lifetime.Token);

            var token = linkedCts.Token;

            session.Output.WriteLine(
                "Unity Commander Internal Console ready.");

            while (!token.IsCancellationRequested)
            {
                var line = await session.Input.ReadLineAsync(token);

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

                await _dispatcher.ExecuteCommandAsync(
                    name,
                    ctx,
                    token);
            }
        }
    }
}
