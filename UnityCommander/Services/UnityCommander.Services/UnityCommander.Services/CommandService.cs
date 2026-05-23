
using CommandSystem.Abstractions;
using CommandSystem.Gui.Integraion;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityCommander.Services
{
    public class CommandService
    {
        private readonly ICommandRegister _register;
        private readonly IGuiCommandExecutor _executor;
        private readonly IGuiCommandProvider _provider;

        public CommandService(
            ICommandRegister register, 
            IGuiCommandExecutor executor, 
            IGuiCommandProvider commandProvider)
        {
            _register = register;
            _executor = executor;
            _provider = commandProvider;
        }

        public void Register(CommandMetadata metadata, Action<CommandContext> handler)
        {
            _register.Register(metadata, handler);
        }

        public void Register(CommandMetadata metadata, Func<CommandContext, Task<UndoToken>> handler)
        {
            _register.Register(metadata, handler);
        }

        // 🔥 undoable команда
        public void RegisterUndoable(
            CommandMetadata metadata,
            Func<CommandContext, Task<UndoToken>> handler)
        {
            _register.Register(metadata, handler);
        }

        public Task ExecuteAsync(string commandName, CommandContext ctx = default)
        {
            return _executor.ExecuteAsync(commandName, ctx);
        }

        public CommandContext Execute(string commandName)
        {
            return _executor.Execute(commandName);
        }

        public IRegisteredCommand Get(string commandName) => _provider.Get(commandName);
        public IReadOnlyCollection<IRegisteredCommand> GetAll() => _provider.GetAll();
    }
}
