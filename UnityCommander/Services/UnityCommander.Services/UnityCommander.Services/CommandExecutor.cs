
using CommandSystem.Abstractions;
using System;
using System.Threading.Tasks;

namespace UnityCommander.Services
{
    public class CommandExecutionService
    {
        private readonly IGuiCommandExecutor _executor;
        private readonly IServiceProvider _services;

        public CommandExecutionService(
            IGuiCommandExecutor executor,
            IServiceProvider services)
        {
            _executor = executor;
            _services = services;
        }

        public Task ExecuteAsync(string commandName, CommandContext ctx = default)
        {
            if (ctx == null)
            {
                ctx = new CommandContext
                {
                    Name = commandName,
                    Services = _services
                };
            }

            return _executor.ExecuteAsync(commandName, ctx?.Parameter, ctx);
        }

        internal bool CanExecute(string id)
        {
            return true;
        }
    }
}
