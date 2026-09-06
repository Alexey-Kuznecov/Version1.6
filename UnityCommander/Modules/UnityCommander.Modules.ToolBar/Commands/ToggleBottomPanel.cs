
using System;
using System.Windows.Input;
using UnityCommander.Common.Commands;
using UnityCommander.Services;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class ToggleBottomPanel : ICommand
    {
        private CommandExecutionService _commandService;
        
        public event EventHandler CanExecuteChanged;

        public ToggleBottomPanel(CommandExecutionService commandService, string id) 
        {
            _commandService = commandService;
            Id = id;
        }

        public string Id { get; }

        public bool CanExecute(object parameter)
            => true;

        public void Execute(object parameter)
        {
            _commandService.ExecuteAsync(CommandNames.UI.ToggleBottomPanel);
        }
    }
}
