
using Prism.Commands;
using System;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Common.Commands;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class CommandUIService : ICommandUIService
    {
        private readonly CommandExecutionService _commands;
        private readonly CompositeIconResolver _iconResolver;

        public CommandUIService(CompositeIconResolver iconResolver, CommandExecutionService commands)
        {
            _commands = commands;
            _iconResolver = iconResolver;
        }

        public UICommand Create(string id)
        {
            var meta = CommandPresentationProvider.Get(id);

            return new UICommand
            {
                Id = id,

                Title = meta.DisplayName,
                Description = meta.Description,

                Icon = _iconResolver.Resolve(id),

                Command = new DelegateCommand(
                    () => _commands.ExecuteAsync(id),
                    () => _commands.CanExecute(id))
            };
        }

        public UICommand Create<T>(
           string id,
           DelegateCommand<T> command,
           Func<bool> canExecute)
        {
            var meta = CommandPresentationProvider.Get(id);

            return new UICommand
            {
                Id = id,
                Title = meta.DisplayName,
                Description = meta.Description,
                Icon = _iconResolver.Resolve(id),
                Command = command,
                CanExecute = canExecute
            };
        }
    }
}
