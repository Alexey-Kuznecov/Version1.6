
using Prism.Commands;
using UnityCommander.Common.Commands;
using UnityCommander.Services.Interfaces;

namespace UnityCommander.Services
{
    public class CommandUIService : ICommandUIService
    {
        private readonly CommandService _commands;
        private readonly IIconProviderService _icons;

        public CommandUIService(IIconProviderService icons, CommandService commands)
        {
            _commands = commands;
            _icons = icons;
        }

        public UICommand Create(string id)
        {
            var meta = CommandPresentationProvider.Get(id);

            return new UICommand
            {
                Id = id,

                Title = meta.DisplayName,
                Description = meta.Description,

                Icon = _icons.GetIcon(id),

                Command = new DelegateCommand(
                    () => _commands.Execute(id),
                    () => _commands.CanExecute(id))
            };
        }
    }
}
