
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityCommander.CLI.Core;
using UnityCommander.CLI.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace UnityCommander.Commands
{
    [ConsoleCommand("exit", "Выходит из консоли.", "e", "ex")]
    public class ExitCommand : IConsoleCommand
    {
        private IServiceProvider _service;

        public string Name => "exit";
        public IEnumerable<string> Aliases => [ "e", "ex" ];
        public string Description => "Выходит из консоли.";

        public ExitCommand(IServiceProvider services)
        {
            _service = services;
        }

        public Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken = default)
        {
             var lifetime = _service.GetService<ConsoleApplicationLifetime>();
            if (lifetime != null)
            {
                lifetime.Stop();
            }
            return Task.CompletedTask;
        }

        public Task FinalizeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
