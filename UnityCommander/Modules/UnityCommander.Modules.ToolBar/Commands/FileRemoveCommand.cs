
using UnityCommander.Ribbon.Core.Models;
using UnityCommander.Services;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class FileRemoveCommand : IRibbonCommand
    {
        private CommandService _commandService;
        public FileRemoveCommand(CommandService commandService, string id)
        {
            _commandService = commandService;
            Id = id;
        }

        public string Id { get; }

        public bool CanExecute() => true;

        public void Execute()
        {
            _commandService.ExecuteAsync("file.delete");
        }
    }
}
