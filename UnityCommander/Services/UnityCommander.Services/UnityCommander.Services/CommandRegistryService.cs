
using CommandSystem.Abstractions;
using CommandSystem.Gui.Integraion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityCommander.Services
{
    public class CommandRegistryService
    {
        private readonly ICommandRegister _register;
        private readonly IGuiCommandProvider _provider;

        public CommandRegistryService(
            ICommandRegister register, 
            IGuiCommandExecutor executor, 
            IGuiCommandProvider commandProvider)
        {
            _register = register;
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

        public void Register(CommandMetadata metadata, Func<CommandContext, Task> handler)
        {
            _register.Register(metadata, handler);
        }

        public void Register(CommandDefinition commandDefinition)
        {
            _register.Register(commandDefinition.Metadata, commandDefinition.Execute);
        }

        public void RegisterUndoable(CommandDefinition commandDefinition)
        {
            _register.Register(commandDefinition.Metadata, commandDefinition.UndoExecute);
        }

        public IRegisteredCommand Get(string commandName) => _provider.Get(commandName);
        
        public IReadOnlyCollection<IRegisteredCommand> GetAll() => _provider.GetAll();
    }
}
