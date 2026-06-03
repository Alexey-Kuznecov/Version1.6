
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UnityCommander.CLI.Commands;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;

namespace UnityCommander.Commands
{
    [ConsoleCommand("exit", "Выходит из консоли.", "e", "ex")]
    public class ExitCommand : IConsoleCommand
    {
        private readonly ConsoleApplicationLifetime _lifetime;
        public string Name => "exit";
        public IEnumerable<string> Aliases => [ "e", "ex" ];
        public string Description => "Выходит из консоли.";

        public ExitCommand(ConsoleApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                if (_lifetime != null)
                {
                    _lifetime.Stop();
                }
            });

            return Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
