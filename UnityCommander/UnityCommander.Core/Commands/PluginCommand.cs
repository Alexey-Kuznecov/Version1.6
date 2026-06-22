
using PluginSystem.Abstractions.Plugin;
using System;
using System.Windows.Input;

namespace UnityCommander.Core.Commands
{
    public class PluginCommand : ICommand
    {
        private readonly IPluginCommand _command;

        private readonly IPluginContext _context;

        public event EventHandler CanExecuteChanged;

        public PluginCommand(IPluginCommand command, IPluginContext context)
        {
            _command = command;
            _context = context;
        }

        public bool CanExecute(object parameter)
           => true;

        public void Execute(object parameter)
        {
            _command.ExecuteAsync(_context);
        }
    }
}
