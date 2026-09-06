
using CommandSystem.Abstractions;
using CommandSystem.Gui.Integraion;
using System.Collections.Generic;

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
