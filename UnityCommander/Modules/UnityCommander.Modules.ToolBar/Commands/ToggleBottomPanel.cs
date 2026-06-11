
using UnityCommander.Common.Commands;
using UnityCommander.Ribbon.Core.Models;
using UnityCommander.Services;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class ToggleBottomPanel : IRibbonCommand
    {
        private CommandExecutionService _commandService;
        
        public ToggleBottomPanel(CommandExecutionService commandService, string id) 
        {
            _commandService = commandService;
            Id = id;
        }

        public string Id { get; }

        public bool CanExecute() => true;

        public void Execute()
        {
            _commandService.ExecuteAsync(CommandNames.UI.ToggleBottomPanel);
        }
    }
}
