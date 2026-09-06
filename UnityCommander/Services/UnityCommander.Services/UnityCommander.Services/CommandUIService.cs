
using Prism.Commands;
using System;
using UnityCommander.Abstractions.Resources;
using UnityCommander.Common.Commands;
using UnityCommander.Common.Panels;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class CommandUIService : ICommandUIService
    {
        private readonly CommandExecutionService _commands;
        private readonly CompositeIconResolver _iconResolver;
        private readonly FilePanelContext _panelContext;

        public CommandUIService(
            CompositeIconResolver iconResolver, 
            CommandExecutionService commands, 
            FilePanelContext panelContext)
        {
            _commands = commands;
            _iconResolver = iconResolver;
            _panelContext = panelContext;
        }

        public UICommand Create(string id)
        {
            var meta = CommandPresentationProvider.Get(id);

            return new UICommand
            {
                Id = id,

                Title = meta.DisplayName,
                Description = meta.Description,

                CommandParameter = _panelContext,

                IconKey = _iconResolver.Resolve(id).Key,

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
                CommandParameter = _panelContext,
                IconKey = _iconResolver.Resolve(id).Key,
                Command = command,
                CanExecute = canExecute
            };
        }
    }
}
