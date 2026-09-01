
using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Helper;
using UnityCommander.CLI.Integration;
using UnityCommander.CLI.Lifecicle;
using static UnityCommander.Common.Commands.CommandNames;

namespace UnityCommander.Modules.BottomPanel.Console
{
    public sealed class ConsoleCommandLoop
    {
        private readonly IServiceProvider _services;
        private readonly ConsoleApplicationLifetime _lifetime;

        private readonly ConsoleLineExecutor _executor;

        public ConsoleCommandLoop(
            IServiceProvider services,
            ConsoleApplicationLifetime lifetime, 
            ConsoleLineExecutor executor)
        {
            _services = services;
            _lifetime = lifetime;
            _executor = executor;
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

                var result = await _executor.ExecuteAsync(session, line, token);

                if (result.Success)
                {
                    //session.Output.WriteLine("Command executed successfully.");

                    if (result.Directives.HasFlag(CommandExecutionDirective.Startup))
                    {
                        session.Profile.StartupCommand = line;

                        session.Output.WriteLine("Command marked as startup.");
                    }
                }
            }
        }
    }
}
