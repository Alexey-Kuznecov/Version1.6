using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCommander.Ribbon.Core.Models;
using UnityCommander.Services;

namespace UnityCommander.Modules.ToolBar.Commands
{
    public class UndoCommand : IRibbonCommand
    {
        private CommandService _commandService;
        public UndoCommand(CommandService commandService, string id)
        {
            _commandService = commandService;
            Id = id;
        }

        public string Id { get; }

        public bool CanExecute() => true;

        public void Execute()
        {
            _commandService.ExecuteAsync("edit.undo");
        }
    }
}
