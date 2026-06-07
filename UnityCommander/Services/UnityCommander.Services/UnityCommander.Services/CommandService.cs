
using CommandSystem.Abstractions;
using CommandSystem.Gui.Integraion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityCommander.Common.Commands;

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

        public Task ExecuteAsync(string commandName, CommandContext ctx = default)
        {
            return _executor.ExecuteAsync(commandName, ctx);
        }

        //public CommandContext Execute(string commandName, object args = null)
        //{
        //    var ctx = new CommandContext(commandName, args);
        //    return _executor.Execute(commandName, args, ctx);
        //}

        //public CommandContext Execute(string commandName, CommandArguments args)
        //{
        //    var ctx = new CommandContext(commandName, args);
        //    return _executor.Execute(commandName, args, ctx);
        //}

        public IRegisteredCommand Get(string commandName) => _provider.Get(commandName);
        
        public IReadOnlyCollection<IRegisteredCommand> GetAll() => _provider.GetAll();

        internal bool CanExecute(string id)
        {
            return true;
        }
    }
}
