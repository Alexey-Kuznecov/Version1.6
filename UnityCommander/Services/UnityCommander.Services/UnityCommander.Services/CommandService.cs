
using CommandSystem.Abstractions;
using System;
using System.Threading.Tasks;

namespace UnityCommander.Services
{
    public class CommandService
    {
        private readonly ICommandRegister _register;
        private readonly IGuiCommandExecutor _executor;

        public CommandService(ICommandRegister register, IGuiCommandExecutor executor)
        {
            _register = register;
            _executor = executor;
        }

        public void Register(CommandMetadata metadata, Action<CommandContext> handler)
        {
            _register.Register(metadata, handler);
        }

        public void Register(CommandMetadata metadata, Func<CommandContext, Task> handler)
        {
            _register.Register(metadata, handler);
        }

        public Task ExecuteAsync(string commandName, CommandContext ctx)
        {
            return _executor.ExecuteAsync(commandName, ctx);
        }

        public CommandContext Execute(string commandName)
        {
            return _executor.Execute(commandName);
        }
    }
}
