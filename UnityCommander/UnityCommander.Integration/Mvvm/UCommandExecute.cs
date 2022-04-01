using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace UnityCommander.Integration.Mvvm
{
    public class PluginCommand : ICommand
    {
        private Action<string> command;

        public PluginCommand(Action<string> cmd)
        {
            this.command = cmd;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var param = (object[])parameter;
            this.command((string)param[0]);
        }

        public Delegate GetCommand()
        {
            return command;
        }
    }
}
